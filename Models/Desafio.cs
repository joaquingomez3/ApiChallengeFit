public class Desafio {
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int Puntos { get; set; }
    public int? IdEntrenador { get; set; }
    public Usuario Entrenador { get; set; }
}
