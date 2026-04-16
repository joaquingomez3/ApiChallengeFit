namespace ApiChallengeFit.Repository.IRepository
{
    public interface IRepositoryRutina
    {
        IList<Rutina> ObtenerPorEntrenador(int idEntrenador);
        IList<Rutina> ObtenerPorAlumno(int idAlumno);
        int Alta(Rutina rutina);
        RutinaEjercicio? ObtenerRutinaEjercicioPorId(int id);
        int EliminarRutinaEjercicio(RutinaEjercicio rutinaEjercicio);

        // Métodos para Alumno
        Rutina? ObtenerRutinaDelDia(int idAlumno);
        int IniciarRutina(int idAlumno, int idRutina);
        int CompletarRutina(int idAlumno, int idRutina);
        int CompletarEjercicio(int idAlumno, int idRutinaEjercicio);

        // Métodos para Entrenamiento (Creación de Rutina)
        IList<Ejercicio> BuscarEjercicios(string nombre);

        // Asignación de rutina a alumno
        int AsignarRutina(int idEntrenador, int idAlumno, int idRutina);

        // Edición de rutina (Entrenador)
        Rutina? ObtenerRutinaPorId(int idRutina);
        int AgregarEjercicioARutina(RutinaEjercicio rutinaEjercicio);
        int EditarRutinaEjercicio(int idRutinaEjercicio, int series, int repeticiones);
    }
}
