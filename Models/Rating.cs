using System.ComponentModel.DataAnnotations;
using RecetArreAPI2.Models;

namespace RecetArreAPI2.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5)] 
        public int Calificacion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación con el Usuario (quien califica)
        [Required]
        public string UsuarioId { get; set; } = default!;
        public ApplicationUser Usuario { get; set; } = default!;

        // Relación con la Receta (la que es calificada)
        [Required]
        public int RecetaId { get; set; }
        public Receta Receta { get; set; } = default!;
    }
}