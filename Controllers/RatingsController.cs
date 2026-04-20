using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecetArreAPI2.Context;
using RecetArreAPI2.DTOs.Ratings;
using RecetArreAPI2.Models;

namespace RecetArreAPI2.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;

        // en este constructor se inyecta el Context, AutoMapper y el gestor de usuarios de Identity
        public RatingsController(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        // Aqui se obtienen todas las calificaciones de una receta especifica
        [HttpGet("receta/{recetaId:int}")]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatingsByReceta(int recetaId)
        {
            // Se busca en la tabla Ratings todas las calificaciones que coincidan con el ID de la receta
            var ratings = await context.Ratings
                .Where(r => r.RecetaId == recetaId)
                .OrderByDescending(r => r.FechaCreacion) // Las mas recientes primero
                .ToListAsync();

            // Se mapea la lista de entidades a una lista de DTOs para enviarlos al cliente
            return Ok(mapper.Map<List<RatingDto>>(ratings));
        }

        // Aqui se obtiene una calificacion individual por su ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RatingDto>> GetRating(int id)
        {
            var rating = await context.Ratings.FirstOrDefaultAsync(r => r.Id == id);

            if (rating == null)
            {
                return NotFound(new { mensaje = "Calificación no encontrada" });
            }

            return Ok(mapper.Map<RatingDto>(rating));
        }

        // Crear una nueva calificacion
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Solo usuarios logueados
        public async Task<ActionResult<RatingDto>> CreateRating(RatingCreacionDto ratingCreacionDto)
        {
            // 1. Se extrae el ID del usuario desde el Token JWT que viene en la peticion
            var usuarioId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { mensaje = "Usuario no autenticado" });
            }

            // 2. ValidaR que la receta que se quiere calificar realmente exista
            var existeReceta = await context.Recetas.AnyAsync(r => r.Id == ratingCreacionDto.RecetaId);
            if (!existeReceta)
            {
                return BadRequest(new { mensaje = "La receta especificada no existe" });
            }

            // 3. Verificar si el usuario ya califico la receta anteriormente
            var yaCalifico = await context.Ratings
                .AnyAsync(r => r.RecetaId == ratingCreacionDto.RecetaId && r.UsuarioId == usuarioId);

            if (yaCalifico)
            {
                return BadRequest(new { mensaje = "Receta ya Calificada" });
            }

            // 4. Mapear el DTO de creacion a la entidad Rating
            var rating = mapper.Map<Rating>(ratingCreacionDto);

            // 5. Asignar manualmente los datos que no vienen en el DTO (autor y fecha)
            rating.UsuarioId = usuarioId;
            rating.FechaCreacion = DateTime.UtcNow;

            // 6. Guardar en la base de datos
            context.Ratings.Add(rating);
            await context.SaveChangesAsync();

            // 7. Retornar el objeto creado usando la ruta de GetRating
            return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, mapper.Map<RatingDto>(rating));
        }

        // Actualizar una calificacion existente (Cambiar las estrellas)
        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateRating(int id, RatingModificacionDto ratingModificacionDto)
        {
            // 1. Obtener el ID del usuario actual
            var usuarioId = userManager.GetUserId(User);

            // 2. Buscar la calificacion en la base de datos
            var ratingDB = await context.Ratings.FirstOrDefaultAsync(r => r.Id == id);

            if (ratingDB == null)
            {
                return NotFound(new { mensaje = "Calificacion no encontrada" });
            }

            // 3. Aqui es para que solo el que puso la calificacion pueda modifiacarla
            if (ratingDB.UsuarioId != usuarioId)
            {
                return Forbid(); // Retorna 403 Prohibido
            }

            // 4. Mapear los cambios del DTO a la entidad de la DB
            mapper.Map(ratingModificacionDto, ratingDB);

            // 5. Marcar como modificado y guardar
            context.Ratings.Update(ratingDB);
            await context.SaveChangesAsync();

            return Ok(new { mensaje = "Calificacion actualizada", data = mapper.Map<RatingDto>(ratingDB) });
        }

        // Eliminar una calificacion
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var usuarioId = userManager.GetUserId(User);

            var rating = await context.Ratings.FirstOrDefaultAsync(r => r.Id == id);
            if (rating == null)
            {
                return NotFound(new { mensaje = "Calificacion no encontrada" });
            }

            // Esto es para que solo el dueño pueda borrar su calificacion
            if (rating.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            context.Ratings.Remove(rating);
            await context.SaveChangesAsync();

            return Ok(new { mensaje = "Calificacion eliminada correctamente" });
        }
    }
}