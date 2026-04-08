using System.ComponentModel.DataAnnotations;

public class CrearUsuarioRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Clave { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    public string Rol { get; set; } // "Entrenador" | "Alumno"

    // Solo para alumnos: texto descriptivo del objetivo
    public string? Objetivo { get; set; }

    // Solo para alumnos: IDs de la tabla objetivos (tabla pivote objetivo_alumno)
    public List<int>? ObjetivoIds { get; set; }

    // Solo para entrenadores: IDs de la tabla especialidades (tabla pivote especialidad_entrenador)
    public List<int>? EspecialidadIds { get; set; }
}
