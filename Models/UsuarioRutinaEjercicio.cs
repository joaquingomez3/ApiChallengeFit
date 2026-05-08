using System.ComponentModel.DataAnnotations.Schema;

public class UsuarioRutinaEjercicio {
    public int Id { get; set; }

    public int IdUsuarioRutina { get; set; }
    [ForeignKey("IdUsuarioRutina")]
    public UsuarioRutina UsuarioRutina { get; set; }

    public int IdRutinaEjercicio { get; set; }
    [ForeignKey("IdRutinaEjercicio")]
    public RutinaEjercicio RutinaEjercicio { get; set; }

    public bool Completado { get; set; } = false;
}
