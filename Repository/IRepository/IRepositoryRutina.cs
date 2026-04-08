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

        // Métodos para Entrenamiento (Creación de Rutina)
        IList<Ejercicio> BuscarEjercicios(string nombre);
    }
}
