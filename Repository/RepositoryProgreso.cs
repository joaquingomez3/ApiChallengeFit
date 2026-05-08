using ApiChallengeFit.Data;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiChallengeFit.Repository;

public class RepositoryProgreso : IRepositoryProgreso
{
    private readonly AppDbContext contexto;

    public RepositoryProgreso(AppDbContext db)
    {
        contexto = db;
    }

    // Progreso semanal: porcentaje de rutinas completadas en la semana actual
    public object ObtenerProgresoSemanal(int idAlumno)
    {
        // Calcular inicio de la semana (lunes)
        var hoy = DateTime.Now.Date;
        var diasDesdeInicioSemana = ((int)hoy.DayOfWeek + 6) % 7; // Lunes = 0
        var inicioSemana = hoy.AddDays(-diasDesdeInicioSemana);
        var finSemana = inicioSemana.AddDays(7);

        // Total de rutinas asignadas al alumno
        var totalAsignadas = contexto.UsuarioRutinas
            .Count(ur => ur.IdUsuario == idAlumno);

        // Rutinas completadas esta semana
        var completadasSemana = contexto.UsuarioRutinas
            .Count(ur => ur.IdUsuario == idAlumno
                && ur.Completado
                && ur.FechaFinalizacion >= inicioSemana
                && ur.FechaFinalizacion < finSemana);

        // Rutinas que se esperaban hacer esta semana (las asignadas que caen en la semana)
        var asignadasSemana = contexto.UsuarioRutinas
            .Count(ur => ur.IdUsuario == idAlumno
                && ur.FechaAsignacion >= inicioSemana
                && ur.FechaAsignacion < finSemana);

        var totalSemana = asignadasSemana > 0 ? asignadasSemana : totalAsignadas;
        var porcentaje = totalSemana > 0
            ? (int)Math.Round((double)completadasSemana / totalSemana * 100)
            : 0;

        return new
        {
            Porcentaje = porcentaje,
            CompletadasSemana = completadasSemana,
            TotalSemana = totalSemana
        };
    }

    // Progreso general: porcentaje total, cantidad de rutinas completadas y desafíos terminados
    public object ObtenerProgresoGeneral(int idAlumno)
    {
        // Total de rutinas asignadas y completadas
        var totalRutinas = contexto.UsuarioRutinas
            .Count(ur => ur.IdUsuario == idAlumno);

        var rutinasCompletadas = contexto.UsuarioRutinas
            .Count(ur => ur.IdUsuario == idAlumno && ur.Completado);

        // Total de desafíos asignados y completados
        var totalDesafios = contexto.DesafioUsuarios
            .Count(du => du.IdUsuario == idAlumno);

        var desafiosCompletados = contexto.DesafioUsuarios
            .Count(du => du.IdUsuario == idAlumno && du.Completado);

        // Porcentaje general (promedio entre rutinas y desafíos)
        var totalItems = totalRutinas + totalDesafios;
        var completadosItems = rutinasCompletadas + desafiosCompletados;
        var porcentajeGeneral = totalItems > 0
            ? (int)Math.Round((double)completadosItems / totalItems * 100)
            : 0;

        return new
        {
            PorcentajeGeneral = porcentajeGeneral,
            RutinasCompletadas = rutinasCompletadas,
            TotalRutinas = totalRutinas,
            DesafiosCompletados = desafiosCompletados,
            TotalDesafios = totalDesafios
        };
    }

    // Rendimiento: datos de los últimos 7 días (rutinas completadas por día)
    public object ObtenerRendimiento(int idAlumno)
    {
        var hoy = DateTime.Now.Date;
        var hace7Dias = hoy.AddDays(-6); // Incluye hoy + 6 días atrás = 7 días

        // Obtener las rutinas completadas en los últimos 7 días agrupadas por fecha
        var datos = contexto.UsuarioRutinas
            .Where(ur => ur.IdUsuario == idAlumno
                && ur.Completado
                && ur.FechaFinalizacion >= hace7Dias
                && ur.FechaFinalizacion < hoy.AddDays(1))
            .ToList() // Materializar para agrupar en memoria
            .GroupBy(ur => ur.FechaFinalizacion!.Value.Date)
            .Select(g => new
            {
                Fecha = g.Key.ToString("yyyy-MM-dd"),
                RutinasCompletadas = g.Count()
            })
            .OrderBy(d => d.Fecha)
            .ToList();

        // Llenar los días sin actividad con 0
        var rendimiento = new List<object>();
        for (int i = 0; i < 7; i++)
        {
            var fecha = hace7Dias.AddDays(i);
            var fechaStr = fecha.ToString("yyyy-MM-dd");
            var dia = datos.FirstOrDefault(d => d.Fecha == fechaStr);
            rendimiento.Add(new
            {
                Fecha = fechaStr,
                Dia = fecha.ToString("ddd"),
                RutinasCompletadas = dia?.RutinasCompletadas ?? 0
            });
        }

        return new
        {
            Periodo = $"{hace7Dias:yyyy-MM-dd} a {hoy:yyyy-MM-dd}",
            Datos = rendimiento
        };
    }

    // Obtiene el progreso desglosado por rutina de un alumno
    // Calcula el porcentaje basándose en los ejercicios completados de la tabla rutinaejercicios
    public object? ObtenerRutinasConProgresoPorAlumno(int idEntrenador, int idAlumno, string estado)
    {
        // 1. Validar que el alumno pertenezca al entrenador logueado
        var alumnoValido = contexto.Usuarios.Any(u => u.Id == idAlumno && u.EntrenadorId == idEntrenador && u.Rol == "Alumno");
        if (!alumnoValido)
        {
            return null;
        }

        // 2. Traer las asignaciones de rutinas para este alumno
        var query = contexto.UsuarioRutinas
            .Include(ur => ur.Rutina)
            .Where(ur => ur.IdUsuario == idAlumno);

        // 3. Filtrar por estado
        if (estado.ToLower() == "completadas")
        {
            query = query.Where(ur => ur.Completado == true);
        }
        else if (estado.ToLower() == "activas")
        {
            query = query.Where(ur => ur.Completado == false);
        }

        var rutinasAsignadas = query.ToList();

        // 4. Calcular progreso basado en ejercicios completados de rutinaejercicios
        var resultado = new List<object>();

        foreach (var asignacion in rutinasAsignadas)
        {
            // Contar ejercicios totales y completados POR ALUMNO
            var totalEjercicios = contexto.UsuarioRutinaEjercicios
                .Count(ure => ure.IdUsuarioRutina == asignacion.Id);

            var ejerciciosCompletados = contexto.UsuarioRutinaEjercicios
                .Count(ure => ure.IdUsuarioRutina == asignacion.Id && ure.Completado);

            int porcentaje;

            if (asignacion.Completado)
            {
                porcentaje = 100;
            }
            else
            {
                porcentaje = totalEjercicios > 0
                    ? (int)Math.Round((double)ejerciciosCompletados / totalEjercicios * 100)
                    : 0;
            }

            resultado.Add(new
            {
                IdRutina = asignacion.IdRutina,
                Nombre = asignacion.Rutina?.Nombre,
                Nivel = asignacion.Rutina?.Nivel,
                Duracion = asignacion.Rutina?.Duracion,
                Completado = asignacion.Completado,
                FechaAsignacion = asignacion.FechaAsignacion,
                Porcentaje = porcentaje,
                EjerciciosCompletados = ejerciciosCompletados,
                TotalEjercicios = totalEjercicios
            });
        }

        return resultado;
    }
}
