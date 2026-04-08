using ApiChallengeFit.Data;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiChallengeFit.Repository;

public class RepositoryRutina : IRepositoryRutina
{
    private readonly AppDbContext contexto;

    public RepositoryRutina(AppDbContext db)
    {
        contexto = db;
    }

    // Devuelve todas las rutinas creadas por el entrenador con el id dado
    public IList<Rutina> ObtenerPorEntrenador(int idEntrenador)
    {
        return contexto.Rutinas
            .Where(r => r.IdEntrenador == idEntrenador)
            .Include(r => r.RutinaEjercicios)
                .ThenInclude(re => re.Ejercicio)
            .ToList();
    }

    // Devuelve todas las rutinas asignadas a un alumno
    public IList<Rutina> ObtenerPorAlumno(int idAlumno)
    {
        // Primero obtenemos los IDs de rutinas asignadas al alumno
        var idsRutinas = contexto.UsuarioRutinas
            .Where(ur => ur.IdUsuario == idAlumno)
            .Select(ur => ur.IdRutina)
            .ToList();

        // Luego consultamos las rutinas con sus ejercicios incluidos
        return contexto.Rutinas
            .Where(r => idsRutinas.Contains(r.Id))
            .Include(r => r.RutinaEjercicios)
                .ThenInclude(re => re.Ejercicio)
            .ToList();
    }

    // Inserta una nueva rutina y devuelve las filas afectadas
    public int Alta(Rutina rutina)
    {
        contexto.Rutinas.Add(rutina);
        return contexto.SaveChanges();
    }

    // Obtiene un RutinaEjercicio por su Id, incluyendo la Rutina asociada
    public RutinaEjercicio? ObtenerRutinaEjercicioPorId(int id)
    {
        return contexto.RutinaEjercicios
            .Include(re => re.Rutina)
            .FirstOrDefault(re => re.Id == id);
    }

    // Elimina un RutinaEjercicio y devuelve filas afectadas
    public int EliminarRutinaEjercicio(RutinaEjercicio rutinaEjercicio)
    {
        contexto.RutinaEjercicios.Remove(rutinaEjercicio);
        return contexto.SaveChanges();
    }

    // Devuelve la rutina del día para el alumno (la primera asignada que no fue completada)
    // Incluye los ejercicios con su detalle
    public Rutina? ObtenerRutinaDelDia(int idAlumno)
    {
        var usuarioRutina = contexto.UsuarioRutinas
            .Where(ur => ur.IdUsuario == idAlumno && !ur.Completado)
            .Include(ur => ur.Rutina)
                .ThenInclude(r => r.RutinaEjercicios)
                    .ThenInclude(re => re.Ejercicio)
            .OrderBy(ur => ur.FechaAsignacion)
            .FirstOrDefault();

        return usuarioRutina?.Rutina;
    }

    // Registra que el alumno inició una rutina (crea un Progreso)
    public int IniciarRutina(int idAlumno, int idRutina)
    {
        // Verificar que la rutina está asignada al alumno
        var asignacion = contexto.UsuarioRutinas
            .FirstOrDefault(ur => ur.IdUsuario == idAlumno && ur.IdRutina == idRutina);

        if (asignacion == null)
            return -1; // No está asignada

        // Verificar que no haya un progreso activo (no completado) para esta rutina
        var progresoExistente = contexto.Progresos
            .FirstOrDefault(p => p.IdUsuario == idAlumno && p.IdRutina == idRutina && !p.Completado);

        if (progresoExistente != null)
            return -2; // Ya hay un progreso activo

        var progreso = new Progreso
        {
            IdUsuario = idAlumno,
            IdRutina = idRutina,
            FechaRegistro = DateTime.Now,
            Completado = false,
            Estadisticas = "{}"
        };

        contexto.Progresos.Add(progreso);
        return contexto.SaveChanges();
    }

    // Marca la rutina como completada para el alumno
    public int CompletarRutina(int idAlumno, int idRutina)
    {
        // Marcar el UsuarioRutina como completado
        var asignacion = contexto.UsuarioRutinas
            .FirstOrDefault(ur => ur.IdUsuario == idAlumno && ur.IdRutina == idRutina && !ur.Completado);

        if (asignacion == null)
            return -1;

        asignacion.Completado = true;
        asignacion.FechaFinalizacion = DateTime.Now;

        // Marcar el Progreso activo como completado
        var progreso = contexto.Progresos
            .FirstOrDefault(p => p.IdUsuario == idAlumno && p.IdRutina == idRutina && !p.Completado);

        if (progreso != null)
        {
            progreso.Completado = true;
        }

        return contexto.SaveChanges();
    }

    // Busca ejercicios por nombre (autocompletado)
    public IList<Ejercicio> BuscarEjercicios(string nombre)
    {
        return contexto.Ejercicios
            .Where(e => e.Nombre.Contains(nombre))
            .Take(15) // Limitar resultados para no sobrecargar el cliente
            .ToList();
    }
}
