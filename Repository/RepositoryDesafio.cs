using ApiChallengeFit.Data;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiChallengeFit.Repository;

public class RepositoryDesafio : IRepositoryDesafio
{
    private readonly AppDbContext contexto;

    public RepositoryDesafio(AppDbContext db)
    {
        contexto = db;
    }

    // Devuelve todos los desafíos creados por el entrenador
    public IList<Desafio> ObtenerPorEntrenador(int idEntrenador)
    {
        return contexto.Desafios
            .Where(d => d.IdEntrenador == idEntrenador)
            .ToList();
    }

    // Devuelve todos los desafíos asignados a un alumno
    public IList<Desafio> ObtenerPorAlumno(int idAlumno)
    {
        return contexto.DesafioUsuarios
            .Where(du => du.IdUsuario == idAlumno)
            .Include(du => du.Desafio)
            .Select(du => du.Desafio)
            .ToList();
    }

    // Inserta un nuevo desafío y devuelve filas afectadas
    public int Alta(Desafio desafio)
    {
        contexto.Desafios.Add(desafio);
        return contexto.SaveChanges();
    }

    // Devuelve los desafíos asignados al alumno con su progreso (DesafioUsuario con Desafio incluido)
    public IList<DesafioUsuario> ObtenerDesafiosConProgreso(int idAlumno)
    {
        return contexto.DesafioUsuarios
            .Where(du => du.IdUsuario == idAlumno)
            .Include(du => du.Desafio)
            .ToList();
    }

    // Obtiene un DesafioUsuario por su Id
    public DesafioUsuario? ObtenerDesafioUsuarioPorId(int id)
    {
        return contexto.DesafioUsuarios
            .Include(du => du.Desafio)
            .FirstOrDefault(du => du.Id == id);
    }

    // Actualiza el progreso de un DesafioUsuario
    public int ActualizarProgreso(DesafioUsuario desafioUsuario)
    {
        contexto.DesafioUsuarios.Update(desafioUsuario);
        return contexto.SaveChanges();
    }

    // Asigna un desafío a un alumno
    // Retorna: -1 si el desafío no existe o no pertenece al entrenador
    //          -2 si el alumno no existe o no está vinculado al entrenador
    //          -3 si el alumno ya tiene este desafío asignado
    //          > 0 si se asignó correctamente
    public int AsignarDesafio(int idEntrenador, int idAlumno, int idDesafio)
    {
        // Verificar que el desafío existe y pertenece al entrenador
        var desafio = contexto.Desafios.FirstOrDefault(d => d.Id == idDesafio && d.IdEntrenador == idEntrenador);
        if (desafio == null)
            return -1;

        // Verificar que el alumno existe, es "Alumno" y está vinculado al entrenador
        var alumno = contexto.Usuarios.FirstOrDefault(u => u.Id == idAlumno && u.Rol == "Alumno" && u.EntrenadorId == idEntrenador);
        if (alumno == null)
            return -2;

        // Verificar que no tenga el mismo desafío ya asignado
        var yaAsignado = contexto.DesafioUsuarios
            .Any(du => du.IdUsuario == idAlumno && du.IdDesafio == idDesafio);
        if (yaAsignado)
            return -3;

        var desafioUsuario = new DesafioUsuario
        {
            IdUsuario = idAlumno,
            IdDesafio = idDesafio,
            Progreso = 0,
            Completado = false,
            FechaAsignado = DateTime.Now
        };

        contexto.DesafioUsuarios.Add(desafioUsuario);
        return contexto.SaveChanges();
    }
}
