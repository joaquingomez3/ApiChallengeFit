using System.ComponentModel.DataAnnotations.Schema;

public class SolicitudVinculacion
{
    public int Id { get; set; }

    public int IdAlumno { get; set; }
    [ForeignKey("IdAlumno")]
    public Usuario? Alumno { get; set; }

    public int IdEntrenador { get; set; }
    [ForeignKey("IdEntrenador")]
    public Usuario? Entrenador { get; set; }

    public string Estado { get; set; } = "Pendiente"; // "Pendiente" | "Aceptada" | "Rechazada"
    public DateTime FechaSolicitud { get; set; } = DateTime.Now;
    public DateTime? FechaRespuesta { get; set; }
}
