using ApiChallengeFit.Data;
using ApiChallengeFit.Repository.IRepository;

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
    
}