
using System.ComponentModel.DataAnnotations;
namespace RecetArreAPI2.DTOs.Ratings
{
    public class RatingDto
    {
        public int Id { get; set; }
        public int Calificacion { get; set; }
        public int RecetaId { get; set; }
        public string UsuarioId { get; set; } = default!;
        public DateTime FechaCreacion { get; set; }
    }

    public class RatingCreacionDto
    {
        [Required]
        [Range(1, 5)]
        public int Calificacion { get; set; }

        [Required]
        public int RecetaId { get; set; }

    }

    public class RatingModificacionDto
    {
        [Required]
        [Range(1, 5)]
        public int Calificacion { get; set; }

        [Required]
        public int RecetaId { get; set; }

    }




}
