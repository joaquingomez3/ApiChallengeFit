public class Especialidad
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public ICollection<EspecialidadEntrenador>? EspecialidadEntrenadores { get; set; }
}
