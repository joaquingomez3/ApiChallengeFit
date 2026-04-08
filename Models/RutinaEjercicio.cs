using System.ComponentModel.DataAnnotations.Schema;

public class RutinaEjercicio {
    public int Id { get; set; }

    public int IdRutina { get; set; }
    [ForeignKey("IdRutina")]
    public Rutina? Rutina { get; set; }

    public int IdEjercicio { get; set; }
    [ForeignKey("IdEjercicio")]
    public Ejercicio? Ejercicio { get; set; }

    public int Series { get; set; }
    public int Repeticiones { get; set; }
    public bool Completado { get; set; } = false; // Para el alumno
}