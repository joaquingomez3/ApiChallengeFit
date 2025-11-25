public class Rutina {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Nivel { get; set; }
    public string Descripcion { get; set; }
    public int Duracion { get; set; }
    public int? IdEntrenador { get; set; }
    public Usuario Entrenador { get; set; }
    public ICollection<RutinaEjercicio> RutinaEjercicios { get; set; }
}