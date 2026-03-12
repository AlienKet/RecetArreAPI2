using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecetArreAPI2.Models
{
    public class Categoria
    {
        
  //"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImFsaWVuMTIzQGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFsaWVuMTIzQGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMTM2OWM5MjYtNjE3Zi00Yjg4LTkxMjctNGFjOGE2YmJlMGViIiwic3ViIjoiMTM2OWM5MjYtNjE3Zi00Yjg4LTkxMjctNGFjOGE2YmJlMGViIiwiZXhwIjoxNzc1OTQ5NTIzfQ.57UNig63h_xgFBWibXA6OBXJbH_eizNOS84D1mwCvBE",
  
        //"expiracion": "2026-04-11T23:18:43.4484072Z",
  //"usuarioId": "1369c926-617f-4b88-9127-4ac8a6bbe0eb"

        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nombre { get; set; } = default!;
        
        [StringLength(500)]
        public string? Descripcion { get; set; }

        public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;

        // Relación con ApplicationUser (quién creó la categoría)
        [ForeignKey("ApplicationUser")]
        public string? CreadoPorUsuarioId { get; set; }


        public ApplicationUser? CreadoPorUsuario { get; set; }

        public ICollection<Receta> Recetas { get; set; } = new List<Receta>();
    }
}
