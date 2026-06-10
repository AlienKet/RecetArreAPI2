using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecetArreAPI2.Context;
using RecetArreAPI2.DTOs.Recetas;
using RecetArreAPI2.Models;

namespace RecetArreAPI2.Controllers
{
    [ApiController]//indica que esta clase es un controlador de API,
     //lo que habilita características como la validación automática del modelo
     //y la serialización de respuestas
    [Route("api/[controller]")]
    public class RecetasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;

        public RecetasController(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]//aqui se puede agregar un filtro para mostrar solo las recetas publicadas o las del usuario autenticado
        public async Task<ActionResult<IEnumerable<RecetaDto>>> GetRecetas()
        {
            var recetas = await context.Recetas
                .Include(r => r.Categorias)
                .Include(r => r.Ingredientes)
                .Include(r => r.Ratings)
                .OrderByDescending(r => r.CreadoUtc)
                .ToListAsync();

            return Ok(mapper.Map<List<RecetaDto>>(recetas));
        }

        [HttpGet("filtrar/categorias")]//aqui se filtran las categorías por los ids enviados en la query string,
        public async Task<ActionResult<IEnumerable<RecetaDto>>> FiltrarPorCategorias([FromQuery] List<int> categoriaIds)
        {
            if (categoriaIds == null || categoriaIds.Count == 0)
            {
                return BadRequest(new { mensaje = "Debe enviar al menos un id de categoría" });
            }

            var ids = categoriaIds.Distinct().ToList();
            var recetas = await context.Recetas
                .Include(r => r.Categorias)
                .Include(r => r.Ingredientes)
                .Include(r => r.Ratings)
                .Where(r => r.Categorias.Any(c => ids.Contains(c.Id)))
                .OrderByDescending(r => r.CreadoUtc)
                .ToListAsync();

            return Ok(mapper.Map<List<RecetaDto>>(recetas));
        }

        [HttpGet("filtrar/ingredientes")]//aqui se puede agregar un filtro para mostrar solo las recetas publicadas o las del usuario autenticado
        public async Task<ActionResult<IEnumerable<RecetaDto>>> FiltrarPorIngredientes([FromQuery] List<int> ingredienteIds)
        {
            if (ingredienteIds == null || ingredienteIds.Count == 0)
            {
                return BadRequest(new { mensaje = "Debe enviar al menos un id de ingrediente" });
            }

            var ids = ingredienteIds.Distinct().ToList();
            var recetas = await context.Recetas
                .Include(r => r.Categorias)
                .Include(r => r.Ingredientes)
                .Include(r => r.Ratings)
                .Where(r => r.Ingredientes.Any(i => ids.Contains(i.Id)))
                .OrderByDescending(r => r.CreadoUtc)
                .ToListAsync();

            return Ok(mapper.Map<List<RecetaDto>>(recetas));
        }

        [HttpGet("{id:int}")]//aqui se puede agregar un filtro para mostrar solo las recetas publicadas o las del usuario autenticado
        public async Task<ActionResult<RecetaDto>> GetReceta(int id)
        {
            var receta = await context.Recetas
                .Include(r => r.Categorias)
                .Include(r => r.Ingredientes)
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receta == null)
            {
                return NotFound(new { mensaje = "Receta no encontrada" });
            }

            return Ok(mapper.Map<RecetaDto>(receta));
        }

        [HttpPost]//aqui se puede agregar un filtro para permitir solo a los usuarios autenticados crear recetas
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RecetaDto>> CreateReceta(RecetaCreacionDto recetaCreacionDto)
        {
            var usuarioId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { mensaje = "Usuario no autenticado" });
            }

            var categoriaIds = recetaCreacionDto.CategoriaIds.Distinct().ToList();
            var categorias = await context.Categorias
                .Where(c => categoriaIds.Contains(c.Id))
                .ToListAsync();

            //si la cantidad de categorías encontradas no coincide con la cantidad de ids enviados,
            //significa que uno o más ids no existen en la base de datos
            if (categorias.Count != categoriaIds.Count)
            {
                return BadRequest(new { mensaje = "Una o más categorías no existen" });
            }

            var ingredienteIds = recetaCreacionDto.IngredienteIds.Distinct().ToList();
            var ingredientes = await context.Ingredientes
                .Where(i => ingredienteIds.Contains(i.Id))
                .ToListAsync();

            if (ingredientes.Count != ingredienteIds.Count)
            {
                return BadRequest(new { mensaje = "Uno o más ingredientes no existen" });
            }

            var receta = mapper.Map<Receta>(recetaCreacionDto);//mapea las propiedades de recetaCreacionDto a una nueva instancia de Receta
            receta.AutorId = usuarioId;
            receta.CreadoUtc = DateTime.UtcNow;
            receta.ModificadoUtc = DateTime.UtcNow;
            receta.Categorias = categorias;
            receta.Ingredientes = ingredientes;

            context.Recetas.Add(receta);
            await context.SaveChangesAsync();

            receta = await context.Recetas
                .Include(r => r.Categorias)
                .Include(r => r.Ingredientes)
                .FirstAsync(r => r.Id == receta.Id);

            return CreatedAtAction(nameof(GetReceta), new { id = receta.Id }, mapper.Map<RecetaDto>(receta));
        }

        [HttpPut("{id:int}")]//aqui se puede agregar un filtro para permitir solo al autor de la receta o a los administradores actualizar la receta
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateReceta(int id, RecetaModificacionDto recetaModificacionDto)
        {
            var usuarioId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { mensaje = "Usuario no autenticado" });
            }

            var receta = await context.Recetas
                .Include(r => r.Categorias)
                .Include(r => r.Ingredientes)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receta == null)
            {
                return NotFound(new { mensaje = "Receta no encontrada" });
            }
            //si el usuario autenticado no es el autor de la receta, se le prohibe actualizarla
            if (receta.AutorId != usuarioId)
            {
                return Forbid();
            }

            var categoriaIds = recetaModificacionDto.CategoriaIds.Distinct().ToList();
            var categorias = await context.Categorias
                .Where(c => categoriaIds.Contains(c.Id))
                .ToListAsync();

            if (categorias.Count != categoriaIds.Count)
            {
                return BadRequest(new { mensaje = "Una o más categorías no existen" });
            }

            var ingredienteIds = recetaModificacionDto.IngredienteIds.Distinct().ToList();
            var ingredientes = await context.Ingredientes
                .Where(i => ingredienteIds.Contains(i.Id))
                .ToListAsync();

            if (ingredientes.Count != ingredienteIds.Count)
            {
                return BadRequest(new { mensaje = "Uno o más ingredientes no existen" });
            }

            mapper.Map(recetaModificacionDto, receta);
            receta.ModificadoUtc = DateTime.UtcNow;
            receta.Categorias = categorias;
            receta.Ingredientes = ingredientes;

            context.Recetas.Update(receta);
            await context.SaveChangesAsync();

            return Ok(new { mensaje = "Receta actualizada exitosamente", data = mapper.Map<RecetaDto>(receta) });
        }

        [HttpDelete("{id:int}")]//aqui se puede agregar un filtro para permitir solo al autor de la receta o a los administradores eliminar la receta
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteReceta(int id)
        {
            var usuarioId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { mensaje = "Usuario no autenticado" });
            }

            var receta = await context.Recetas.FirstOrDefaultAsync(r => r.Id == id);
            if (receta == null)
            {
                return NotFound(new { mensaje = "Receta no encontrada" });
            }

            if (receta.AutorId != usuarioId)
            {
                return Forbid();
            }

            context.Recetas.Remove(receta);
            await context.SaveChangesAsync();

            return Ok(new { mensaje = "Receta eliminada exitosamente" });
        }
    }
}