namespace ApiChallengeFit.Repository.IRepository
{
    public interface IRepositoryUsuario
    {
        // métodos para el repositorio de Usuario 
        Usuario ObtenerPorEmail(string mail);
        int Alta(Usuario usuario);
        IList<Usuario> ObtenerAlumnosConProgreso(int idEntrenador);
    }
};