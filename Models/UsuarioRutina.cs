public class UsuarioRutina {
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public Usuario Usuario { get; set; }
    public int IdRutina { get; set; }
    public Rutina Rutina { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaFinalizacion { get; set; }
    public bool Completado { get; set; }
}