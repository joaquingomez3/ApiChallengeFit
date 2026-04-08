using System.ComponentModel.DataAnnotations.Schema;

public class Progreso {
    public int Id { get; set; }

    public int IdUsuario { get; set; }
    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; }

    public int? IdRutina { get; set; }
    [ForeignKey("IdRutina")]
    public Rutina Rutina { get; set; }

    public DateTime FechaRegistro { get; set; }
    public string Estadisticas { get; set; } // JSON
    public bool Completado { get; set; }
}