using Microsoft.EntityFrameworkCore;

namespace ApiChallengeFit.Data
{
    public class AppDbContext : DbContext //hereda de DbContext
    {
        //constructor que recibe opciones de configuración (cadena de conexión, proveedor MySQL, etc.)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Cada db set representa una tabla en la base de datos
        public DbSet<Desafio> Desafios { get; set; }
        public DbSet<DesafioUsuario> DesafioUsuarios { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<Progreso> Progresos { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }
        public DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioRutina> UsuarioRutinas { get; set; }
    }


}