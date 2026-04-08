using System.ComponentModel.DataAnnotations.Schema;

public class DesafioUsuario {
    public int Id { get; set; }

    public int IdUsuario { get; set; }
    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; }

    public int IdDesafio { get; set; }
    [ForeignKey("IdDesafio")]
    public Desafio Desafio { get; set; }

    public int Progreso { get; set; }
    public bool Completado { get; set; }
    public DateTime FechaAsignado { get; set; }
}