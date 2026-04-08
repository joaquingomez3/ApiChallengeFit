using System.ComponentModel.DataAnnotations.Schema;

public class EspecialidadEntrenador
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }
    [ForeignKey("IdUsuario")]
    public Usuario? Usuario { get; set; }

    public int IdEspecialidad { get; set; }
    [ForeignKey("IdEspecialidad")]
    public Especialidad? Especialidad { get; set; }
}
