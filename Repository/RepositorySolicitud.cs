using ApiChallengeFit.Data;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiChallengeFit.Repository;

public class RepositorySolicitud : IRepositorySolicitud
{
    private readonly AppDbContext contexto;

    public RepositorySolicitud(AppDbContext db)
    {
        contexto = db;
    }

    // Busca entrenadores por nombre (parcial) y devuelve con sus especialidades
    public IList<object> BuscarEntrenadores(string nombre)
    {
        return contexto.Usuarios
            .Where(u => u.Rol == "Entrenador" && u.Nombre.Contains(nombre))
            .Select(u => new
            {
                u.Id,
                u.Nombre,
                u.Email,
                Especialidades = contexto.EspecialidadEntrenadores
                    .Where(ee => ee.IdUsuario == u.Id)
                    .Include(ee => ee.Especialidad)
                    .Select(ee => new
                    {
                        ee.Especialidad!.Id,
                        ee.Especialidad.Nombre
                    })
                    .ToList()
            })
            .ToList<object>();
    }

    // Crea una nueva solicitud de vinculación
    public int CrearSolicitud(SolicitudVinculacion solicitud)
    {
        contexto.Solicitudes.Add(solicitud);
        return contexto.SaveChanges();
    }

    // Obtiene las solicitudes pendientes para un entrenador
    public IList<object> ObtenerPendientesPorEntrenador(int idEntrenador)
    {
        return contexto.Solicitudes
            .Where(s => s.IdEntrenador == idEntrenador && s.Estado == "Pendiente")
            .Include(s => s.Alumno)
            .Select(s => new
            {
                s.Id,
                s.IdAlumno,
                NombreAlumno = s.Alumno != null ? s.Alumno.Nombre : null,
                EmailAlumno = s.Alumno != null ? s.Alumno.Email : null,
                s.FechaSolicitud
            })
            .ToList<object>();
    }

    // Obtiene las solicitudes enviadas por un alumno
    public IList<object> ObtenerPorAlumno(int idAlumno)
    {
        return contexto.Solicitudes
            .Where(s => s.IdAlumno == idAlumno)
            .Include(s => s.Entrenador)
            .Select(s => new
            {
                s.Id,
                s.IdEntrenador,
                NombreEntrenador = s.Entrenador != null ? s.Entrenador.Nombre : null,
                s.Estado,
                s.FechaSolicitud,
                s.FechaRespuesta
            })
            .ToList<object>();
    }

    // Obtiene una solicitud por su Id
    public SolicitudVinculacion? ObtenerPorId(int id)
    {
        return contexto.Solicitudes
            .Include(s => s.Alumno)
            .FirstOrDefault(s => s.Id == id);
    }

    // Acepta la solicitud y vincula al alumno con el entrenador
    public int AceptarSolicitud(SolicitudVinculacion solicitud)
    {
        solicitud.Estado = "Aceptada";
        solicitud.FechaRespuesta = DateTime.Now;

        // Vincular al alumno con el entrenador
        var alumno = contexto.Usuarios.FirstOrDefault(u => u.Id == solicitud.IdAlumno);
        if (alumno != null)
        {
            alumno.EntrenadorId = solicitud.IdEntrenador;
        }

        return contexto.SaveChanges();
    }

    // Rechaza la solicitud
    public int RechazarSolicitud(SolicitudVinculacion solicitud)
    {
        solicitud.Estado = "Rechazada";
        solicitud.FechaRespuesta = DateTime.Now;
        return contexto.SaveChanges();
    }

    // Verifica si ya existe una solicitud pendiente del alumno al entrenador
    public bool ExisteSolicitudPendiente(int idAlumno, int idEntrenador)
    {
        return contexto.Solicitudes
            .Any(s => s.IdAlumno == idAlumno && s.IdEntrenador == idEntrenador && s.Estado == "Pendiente");
    }

    // Verifica si el alumno ya está vinculado a ese entrenador
    public bool EsAlumnoDe(int idAlumno, int idEntrenador)
    {
        var alumno = contexto.Usuarios.FirstOrDefault(u => u.Id == idAlumno);
        return alumno != null && alumno.EntrenadorId == idEntrenador;
    }
}
