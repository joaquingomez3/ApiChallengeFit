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
}
