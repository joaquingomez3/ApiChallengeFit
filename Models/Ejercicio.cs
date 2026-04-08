public class Ejercicio
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string GrupoMuscular { get; set; }
    // La relación con Rutina se maneja a través de RutinaEjercicio
    public ICollection<RutinaEjercicio>? RutinaEjercicios { get; set; }
}
