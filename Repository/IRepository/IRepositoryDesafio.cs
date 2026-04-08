namespace ApiChallengeFit.Repository.IRepository
{
    public interface IRepositoryDesafio
    {
        IList<Desafio> ObtenerPorEntrenador(int idEntrenador);
        IList<Desafio> ObtenerPorAlumno(int idAlumno);
        int Alta(Desafio desafio);

        // Métodos para Alumno
        IList<DesafioUsuario> ObtenerDesafiosConProgreso(int idAlumno);
        DesafioUsuario? ObtenerDesafioUsuarioPorId(int id);
        int ActualizarProgreso(DesafioUsuario desafioUsuario);
    }
}
