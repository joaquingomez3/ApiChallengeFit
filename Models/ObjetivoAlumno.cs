using System.ComponentModel.DataAnnotations.Schema;

public class ObjetivoAlumno
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }
    [ForeignKey("IdUsuario")]
    public Usuario? Usuario { get; set; }

    public int IdObjetivo { get; set; }
    [ForeignKey("IdObjetivo")]
    public Objetivo? Objetivo { get; set; }
}
