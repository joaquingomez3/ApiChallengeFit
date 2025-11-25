public class Progreso {
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public Usuario Usuario { get; set; }
    public int? IdRutina { get; set; }
    public Rutina Rutina { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estadisticas { get; set; } // JSON
    public bool Completado { get; set; }
}