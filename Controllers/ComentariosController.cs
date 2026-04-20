using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecetArreAPI2.Context;
using RecetArreAPI2.DTOs.Comentarios;
using RecetArreAPI2.Models;

namespace RecetArreAPI2.Controllers
{
    [ApiController] // aqui le decimos a net que esta clase va a ser una api hecha y derecha
    [Route("api/[controller]")] // la ruta base va a ser api/comentarios para que el front nos encuentre
    public class ComentariosController : ControllerBase // hereda de controllerbase porque es puritita api, sin vistas
    {
        private readonly ApplicationDbContext context; // esta es la conexion a la base de datos
        private readonly IMapper mapper; // el que convierte los modelos a dtos y viceversa
        private readonly UserManager<ApplicationUser> userManager; // este mero es para saber quien es el usuario logueado

        public ComentariosController( // aqui inyectamos todo lo que ocupamos apenas arranque el controlador
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context; // guardamos la conexion a la base de datos
            this.mapper = mapper; // guardamos el mapeador
            this.userManager = userManager; // guardamos el gestor de usuarios
        }

        // GET: api/comentarios/receta/5
        [HttpGet("receta/{recetaId:int}")] // responde a un get y ocupa de a fuerzas el id de la receta
        public async Task<ActionResult<IEnumerable<ComentarioDto>>> GetComentariosPorReceta(int recetaId)
        {
            // primero checamos si la receta de verdad existe en la base de datos
            var existeReceta = await context.Recetas.AnyAsync(r => r.Id == recetaId);
            if (!existeReceta)
            {
                // si no existe mandamos un 404 con el aviso
                return NotFound(new { mensaje = "Receta no encontrada" });
            }

            // vamos por los comentarios de esa receta y los ordenamos de los mas nuevos a los viejos
            var comentarios = await context.Comentarios
                .Where(c => c.RecetaId == recetaId)
                .OrderByDescending(c => c.CreadoUtc)
                .ToListAsync();

            // convertimos la lista a DTOs y mandamos un 200 OK con los datos
            return Ok(mapper.Map<List<ComentarioDto>>(comentarios));
        }

        // POST: api/comentarios
        [HttpPost] // esto es para guardar algo nuevo
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // aqui solo pasan los que traen su token jwt
        public async Task<ActionResult<ComentarioDto>> CreateComentario(ComentarioCreacionDto comentarioCreacionDto)
        {
            // le preguntamos al sistema quien es el usuario que esta escribiendo
            var usuarioId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                // si no hay usuario pues no lo dejamos comentar
                return Unauthorized(new { mensaje = "Usuario no autenticado" });
            }

            // checamos que la receta exista para no colgar comentarios en el aire
            var recetaExiste = await context.Recetas.AnyAsync(r => r.Id == comentarioCreacionDto.RecetaId);
            if (!recetaExiste)
            {
                return NotFound(new { mensaje = "Receta no encontrada" });
            }

            // aqui transformamos el dto que viene del front a un modelo de comentario real
            var comentario = mapper.Map<Comentario>(comentarioCreacionDto);
            comentario.UsuarioId = usuarioId; // le pegamos el id del usuario que lo escribio
            comentario.CreadoUtc = DateTime.UtcNow; // le ponemos la hora de ahorita en formato universal

            context.Comentarios.Add(comentario); // lo echamos a la lista de la base de datos
            await context.SaveChangesAsync(); // guardamos los cambios neta en la base de datos

            // regresamos un 201 y le decimos donde puede consultar el comentario nuevo
            return CreatedAtAction(nameof(GetComentariosPorReceta), new { recetaId = comentario.RecetaId }, mapper.Map<ComentarioDto>(comentario));
        }

        // PUT: api/comentarios/5
        [HttpPut("{id:int}")] // esto es para cambiarle algo a un comentario que ya existe
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateComentario(int id, ComentarioModificacionDto comentarioModificacionDto)
        {
            // sacamos el id del usuario logueado
            var usuarioId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { mensaje = "Usuario no autenticado" });
            }

            // buscamos el comentario original en la base de datos usando el id
            var comentario = await context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
            if (comentario == null)
            {
                return NotFound(new { mensaje = "Comentario no encontrado" });
            }

            // aqui checamos que el que quiera editar sea el mismo que lo escribio
            if (comentario.UsuarioId != usuarioId)
            {
                // si no es el dueño le mandamos un 403 prohibido
                return Forbid();
            }

            // pasamos los cambios del dto al comentario que ya teniamos
            mapper.Map(comentarioModificacionDto, comentario);
            context.Comentarios.Update(comentario); // avisamos que hubo cambios
            await context.SaveChangesAsync(); // guardamos en la base de datos

            // regresamos un 200 OK con el aviso de que ya quedo
            return Ok(new { mensaje = "Comentario actualizado exitosamente", data = mapper.Map<ComentarioDto>(comentario) });
        }

        // DELETE: api/comentarios/5
        [HttpDelete("{id:int}")] // aqui es para darle cuello al comentario
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            // preguntamos quien es el usuario logueado
            var usuarioId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { mensaje = "Usuario no autenticado" });
            }

            // buscamos el comentario que queremos borrar
            var comentario = await context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
            if (comentario == null)
            {
                return NotFound(new { mensaje = "Comentario no encontrado" });
            }

            // solo el que escribio el comentario puede borrarlo
            if (comentario.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            context.Comentarios.Remove(comentario); // lo sacamos de la base de datos
            await context.SaveChangesAsync(); // aplicamos el borrado real

            return Ok(new { mensaje = "Comentario eliminado exitosamente" });
        }
    }
}