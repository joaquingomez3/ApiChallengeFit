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
        public DbSet<Objetivo> Objetivos { get; set; }
        public DbSet<ObjetivoAlumno> ObjetivoAlumnos { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<EspecialidadEntrenador> EspecialidadEntrenadores { get; set; }
        public DbSet<SolicitudVinculacion> Solicitudes { get; set; }
        public DbSet<UsuarioRutinaEjercicio> UsuarioRutinaEjercicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapear nombres de tablas que no coinciden con la convención de EF Core
            modelBuilder.Entity<EspecialidadEntrenador>().ToTable("especialidad_entrenador");
            modelBuilder.Entity<ObjetivoAlumno>().ToTable("objetivo_alumno");
        }
    }


}