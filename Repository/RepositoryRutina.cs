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

    // Marca un ejercicio de una rutina como completado e influye el progreso general
    public int CompletarEjercicio(int idAlumno, int idRutinaEjercicio)
    {
        var ejercicio = contexto.RutinaEjercicios
            .Include(re => re.Rutina)
            .FirstOrDefault(re => re.Id == idRutinaEjercicio);

        if (ejercicio == null) return -1;

        // Verificar si la rutina correspondiente al ejercicio le está asignada al alumno sin completar
        var asignacion = contexto.UsuarioRutinas
            .FirstOrDefault(ur => ur.IdUsuario == idAlumno && ur.IdRutina == ejercicio.IdRutina && !ur.Completado);

        if (asignacion == null) return -1;

        // Actualizar como completado
        ejercicio.Completado = true;
        contexto.SaveChanges();

        // Calcular progreso general de la rutina
        var totalEjercicios = contexto.RutinaEjercicios.Count(re => re.IdRutina == ejercicio.IdRutina);
        var ejerciciosCompletados = contexto.RutinaEjercicios.Count(re => re.IdRutina == ejercicio.IdRutina && re.Completado);

        if (totalEjercicios > 0 && ejerciciosCompletados >= totalEjercicios)
        {
            // Completar automáticamente y cerrar la rutina dado que se completaron todos los ejercicios
            CompletarRutina(idAlumno, ejercicio.IdRutina);
        }
        else
        {
            // Opcionalmente podemos actualizar el progreso vivo de la rutina actual
            var progresoActivo = contexto.Progresos
                .FirstOrDefault(p => p.IdUsuario == idAlumno && p.IdRutina == ejercicio.IdRutina && !p.Completado);

            if (progresoActivo != null)
            {
                int porcentaje = totalEjercicios > 0 ? (int)Math.Round((double)ejerciciosCompletados / totalEjercicios * 100) : 0;
                progresoActivo.Estadisticas = $"{{\"porcentaje\": {porcentaje}}}";
                contexto.SaveChanges();
            }
        }

        return 1;
    }

    // Busca ejercicios por nombre (autocompletado)
    public IList<Ejercicio> BuscarEjercicios(string nombre)
    {
        return contexto.Ejercicios
            .Where(e => e.Nombre.Contains(nombre))
            .Take(15)
            .ToList();
    }

    // Asigna una rutina a un alumno
    // Retorna: -1 si la rutina no existe o no pertenece al entrenador
    //          -2 si el alumno no existe o no está vinculado al entrenador
    //          -3 si el alumno ya tiene esta rutina asignada (no completada)
    //          > 0 si se asignó correctamente
    public int AsignarRutina(int idEntrenador, int idAlumno, int idRutina)
    {
        // Verificar que la rutina existe y pertenece al entrenador
        var rutina = contexto.Rutinas.FirstOrDefault(r => r.Id == idRutina && r.IdEntrenador == idEntrenador);
        if (rutina == null)
            return -1;

        // Verificar que el alumno existe, es "Alumno" y está vinculado al entrenador
        var alumno = contexto.Usuarios.FirstOrDefault(u => u.Id == idAlumno && u.Rol == "Alumno" && u.EntrenadorId == idEntrenador);
        if (alumno == null)
            return -2;

        // Verificar que no tenga la misma rutina asignada sin completar
        var yaAsignada = contexto.UsuarioRutinas
            .Any(ur => ur.IdUsuario == idAlumno && ur.IdRutina == idRutina && !ur.Completado);
        if (yaAsignada)
            return -3;

        var usuarioRutina = new UsuarioRutina
        {
            IdUsuario = idAlumno,
            IdRutina = idRutina,
            FechaAsignacion = DateTime.Now,
            Completado = false
        };

        contexto.UsuarioRutinas.Add(usuarioRutina);
        return contexto.SaveChanges();
    }

    // Obtiene una rutina por su Id incluyendo ejercicios
    public Rutina? ObtenerRutinaPorId(int idRutina)
    {
        return contexto.Rutinas
            .Include(r => r.RutinaEjercicios)
                .ThenInclude(re => re.Ejercicio)
            .FirstOrDefault(r => r.Id == idRutina);
    }

    // Agrega un ejercicio a una rutina existente
    public int AgregarEjercicioARutina(RutinaEjercicio rutinaEjercicio)
    {
        contexto.RutinaEjercicios.Add(rutinaEjercicio);
        return contexto.SaveChanges();
    }

    // Edita series y repeticiones de un RutinaEjercicio existente
    public int EditarRutinaEjercicio(int idRutinaEjercicio, int series, int repeticiones)
    {
        var re = contexto.RutinaEjercicios
            .Include(x => x.Rutina)
            .FirstOrDefault(x => x.Id == idRutinaEjercicio);

        if (re == null) return -1;

        re.Series = series;
        re.Repeticiones = repeticiones;
        return contexto.SaveChanges();
    }
}
