public class Objetivo
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public ICollection<ObjetivoAlumno>? ObjetivoAlumnos { get; set; }
}
