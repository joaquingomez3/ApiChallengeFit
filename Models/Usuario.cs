public class Usuario {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string ClaveHash { get; set; }
    public string Rol { get; set; } // "Entrenador" | "Alumno"
    public string Objetivo { get; set; }
    public ICollection<Rutina> RutinasCreadas { get; set; }
}