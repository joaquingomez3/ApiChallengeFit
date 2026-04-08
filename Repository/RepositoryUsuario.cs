using ApiChallengeFit.Data;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiChallengeFit.Repository;
public class RepositoryUsuario : IRepositoryUsuario
{
    // Implementación de los métodos para el repositorio de Usuario
    private readonly AppDbContext contexto;  //creo instancia del contexto
    private string? secretKey;

    public RepositoryUsuario(AppDbContext db, IConfiguration configuration)  //constructor
    {
        contexto = db;
        secretKey = configuration.GetValue<string>("TokenAuthentication:SecretKey");
    }
    
    public Usuario ObtenerPorEmail(string mail)
    {
        return contexto.Usuarios.FirstOrDefault(u => u.Email == mail);
    }

    public int Alta(Usuario usuario)
    {
        contexto.Usuarios.Add(usuario);
        return contexto.SaveChanges();
    }

    // Obtiene los alumnos asignados al entrenador con sus rutinas asignadas y progreso
    public IList<Usuario> ObtenerAlumnosConProgreso(int idEntrenador)
    {
        return contexto.Usuarios
            .Where(u => u.EntrenadorId == idEntrenador && u.Rol == "Alumno")
            .Select(u => new Usuario
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                Rol = u.Rol,
                Objetivo = u.Objetivo,
                EntrenadorId = u.EntrenadorId
            })
            .ToList();
    }
}