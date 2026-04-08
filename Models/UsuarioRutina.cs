using System.ComponentModel.DataAnnotations.Schema;

public class UsuarioRutina {
    public int Id { get; set; }

    public int IdUsuario { get; set; }
    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; }

    public int IdRutina { get; set; }
    [ForeignKey("IdRutina")]
    public Rutina Rutina { get; set; }

    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaFinalizacion { get; set; }
    public bool Completado { get; set; }
}