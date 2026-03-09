using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RecetArreAPI2.Models
{
       public class Receta
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nombre { get; set; } = default!;

        [Required]
        [StringLength(600, MinimumLength = 2)]
        publis string Instrucciones { get; set; };

        [ForeignKey("Tiempo")]
        public string idTiempo { get; set; };

        [ForeignKey("Usuario")]
        public string idUsuario { get; set; };

        public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;

    }

}
