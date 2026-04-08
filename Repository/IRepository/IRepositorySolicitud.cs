namespace ApiChallengeFit.Repository.IRepository
{
    public interface IRepositorySolicitud
    {
        IList<object> BuscarEntrenadores(string nombre);
        int CrearSolicitud(SolicitudVinculacion solicitud);
        IList<object> ObtenerPendientesPorEntrenador(int idEntrenador);
        IList<object> ObtenerPorAlumno(int idAlumno);
        SolicitudVinculacion? ObtenerPorId(int id);
        int AceptarSolicitud(SolicitudVinculacion solicitud);
        int RechazarSolicitud(SolicitudVinculacion solicitud);
        bool ExisteSolicitudPendiente(int idAlumno, int idEntrenador);
        bool EsAlumnoDe(int idAlumno, int idEntrenador);
    }
}
