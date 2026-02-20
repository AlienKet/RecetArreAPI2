using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RecetArreAPI2.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        [StringLength(500)]
        public string? Biografia { get; set; }

        [StringLength(300)]
        [Url]
        public string? AvatarUrl { get; set; }

        [StringLength(120)]
        public string? Localidad { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        [StringLength(200)]
        [Url]
        public string? PaginaWebUrl { get; set; }

        [StringLength(200)]
        public string? Dietas { get; set; }

        public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
        public DateTime ModificadoUtc { get; set; } = DateTime.UtcNow;
    }
}
