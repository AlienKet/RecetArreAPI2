using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecetArreAPI2.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }//id ingredientes

        [Required]//obligatorio
        [StringLength(100, MinimumLength = 2)]//nombre ingrediente, 100 caracteres maximo, 2 minimo
        public string Nombre { get; set; } = default!;//nombre del ingrediente

        [Required]//obligatorio
        [StringLength(15)]//unidad de medida del ingrediente, 15 caracteres maximo
        public string UnidadMedida { get; set; } = default!;//unidad de medida del ingrediente
        //default! se usa para indicar que el valor no puede ser null, pero no se asigna un valor inicial, se espera que se asigne antes de usar la propiedad

        [StringLength(100)]//descripcion del ingrediente, 500 caracteres maximo
        public string? Descripcion { get; set; }//Descripcion del ingrediente

        public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;

        // Relación con ApplicationUser (quién creó la categoría)
        [ForeignKey("ApplicationUser")]
        public string? CreadoPorUsuarioId { get; set; }


        public ApplicationUser? CreadoPorUsuario { get; set; }

    }
}
