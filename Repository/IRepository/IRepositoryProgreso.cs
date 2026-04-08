namespace ApiChallengeFit.Repository.IRepository
{
    public interface IRepositoryProgreso
    {
        // Progreso semanal: porcentaje de rutinas completadas esta semana
        object ObtenerProgresoSemanal(int idAlumno);

        // Progreso general: %, total rutinas, total desafíos terminados
        object ObtenerProgresoGeneral(int idAlumno);

        // Rendimiento: datos de los últimos 7 días
        object ObtenerRendimiento(int idAlumno);
    }
}
