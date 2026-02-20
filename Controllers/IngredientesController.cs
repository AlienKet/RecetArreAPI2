using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecetArreAPI2.Context;
using RecetArreAPI2.DTOs.Ingredientes;
using RecetArreAPI2.Models;

namespace RecetArreAPI2.Controllers
{

    [ApiController]//indica que esta clase es un controlador de API, lo que permite que se manejen las solicitudes HTTP y se devuelvan respuestas adecuadas
    [Route("api/[controller]")]//define la ruta base para las solicitudes a este controlador

    public class IngredientesController : ControllerBase//indica que esta clase es un controlador de API que no tiene vistas asociadas
    {
        private readonly ApplicationDbContext context;//esto es para acceder a la base de datos y realizar operaciones CRUD en la entidad Ingrediente
        private readonly IMapper mapper;//esto es para mapear entre las entidades del modelo de datos y los DTOs (Data Transfer Objects) que se utilizan para enviar y recibir datos a través de la API
        private readonly UserManager<ApplicationUser> userManager;//esto es para gestionar los usuarios de la aplicación

        public IngredientesController(//aqui se inyectan las dependencias necesarias para el controlador a través del constructor
            ApplicationDbContext context,//aqui se accede a la base de datos para realizar operaciones CRUD en la entidad Ingrediente
            IMapper mapper,//aqui se mapea entre las entidades del modelo de datos y los DTOs (Data Transfer Objects) que se utilizan para enviar y recibir datos a través de la API
            UserManager<ApplicationUser> userManager)//aqui se gestiona los usuarios de la aplicación
        {
            this.context = context;//aqui se asigna el contexto de la base de datos a la variable de instancia para su uso en los métodos del controlador
            this.mapper = mapper;
            this.userManager = userManager;
        }//fin del constructor

        //Get: api/ingredientes
        [HttpGet]//esto es para indicar que este método responde a solicitudes HTTP GET
                 //HttpGes significa que este método se ejecutará cuando se realice una solicitud GET a la ruta "api/ingredientes"
        public async Task<ActionResult<IEnumerable<IngredienteDto>>> GetIngredientes()//este método devuelve una lista de ingredientes en formato DTO (Data Transfer Object)
        {//async es para indicar que este método es asíncrono, lo que permite que se realicen operaciones de manera no bloqueante, como acceder a la base de datos
            var ingredientes = await context.Ingredientes//aqui se accede a la base de datos para obtener la lista de ingredientes
                .OrderByDescending(i => i.CreadoUtc)//aqui se ordena la lista de ingredientes por la fecha de creación en orden descendente
                .ToListAsync();//aqui se convierte el resultado de la consulta a una lista de ingredientes
            return Ok(mapper.Map<List<IngredienteDto>>(ingredientes));//aqui se mapea la lista de ingredientes a una lista de DTOs y se devuelve como respuesta HTTP con un código de estado 200 (OK)
        }

        // GET: api/ingredientes/{id} es para obtener un ingrediente específico por su ID
        [HttpGet("{id}")]//esto es para indicar que este método responde a solicitudes HTTP GET con un parámetro de ruta "id"
        public async Task<ActionResult<IngredienteDto>> GetIngrediente(int id)//este método devuelve un ingrediente específico en formato DTO (Data Transfer Object) basado en el ID proporcionado
        {
            var ingrediente = await context.Ingredientes.FirstOrDefaultAsync(i => i.Id == id);//aqui se accede a la base de datos para obtener el ingrediente con el ID especificado
            if (ingrediente == null)//aqui se verifica si el ingrediente no fue encontrado en la base de datos
            {
                return NotFound(new { mensaje = "Ingrediente no encontrado" });//si el ingrediente no fue encontrado, se devuelve una respuesta HTTP con un código de estado 404 (Not Found) y un mensaje indicando que el ingrediente no fue encontrado
            }
            return Ok(mapper.Map<IngredienteDto>(ingrediente));//si el ingrediente fue encontrado, se mapea a un DTO y se devuelve como respuesta HTTP con un código de estado 200 (OK)
        }

        //POST: api/ingredientes es para crear un nuevo ingrediente
        [HttpPost]//esto es para indicar que este método responde a solicitudes HTTP POST
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]//este endpoint solo la puede usar los que estan autorizados
        public async Task<ActionResult<IngredienteDto>> CreateIngrediente(IngredienteCreacionDto ingredienteCreacionDto)//este método crea un nuevo ingrediente basado en los datos proporcionados en el DTO de creación
        {
            // Validar que el nombre no esté duplicado
            var existe = await context.Ingredientes
                .AnyAsync(i => i.Nombre.ToLower() == ingredienteCreacionDto.Nombre.ToLower());//aqui se verifica si ya existe un ingrediente con el mismo nombre en la base de datos, ignorando mayúsculas y minúsculas
            if (existe)
            {
                return BadRequest(new { mensaje = "Ya existe un ingrediente con ese nombre" });//si ya existe un ingrediente con el mismo nombre, se devuelve una respuesta HTTP con un código de estado 400 (Bad Request)
            }
            // Obtener el usuario autenticado
            var usuarioId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { mensaje = "Usuario no autenticado" });
            }

            var ingrediente = mapper.Map<Ingrediente>(ingredienteCreacionDto);//aqui se mapea el DTO de creación a una entidad de ingrediente
            ingrediente.CreadoUtc = DateTime.UtcNow;
            ingrediente.CreadoPorUsuarioId = usuarioId;

            context.Ingredientes.Add(ingrediente);//aqui se agrega el nuevo ingrediente a la base de datos
            await context.SaveChangesAsync();//aqui se guardan los cambios en la base de datos
        
            return CreatedAtAction(nameof(GetIngrediente), new { id = ingrediente.Id }, mapper.Map<IngredienteDto>(ingrediente));//aqui se devuelve una respuesta HTTP con un código de estado 201 (Created)

        }

        //PUT: api/ingredientes/{id} es para actualizar un ingrediente existente
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateIngrediente(int id, IngredienteModificacionDto ingredienteModificacionDto)//este método actualiza un ingrediente existente basado en el ID proporcionado y los datos de modificación en el DTO
        {
            var ingrediente = await context.Ingredientes.FirstOrDefaultAsync(i => i.Id == id);//aqui se accede a la base de datos para obtener el ingrediente con el ID especificado
            if (ingrediente == null)//aqui se verifica si el ingrediente no fue encontrado en la base de datos
            {
                return NotFound(new { mensaje = "Ingrediente no encontrado" });//si el ingrediente no fue encontrado, se devuelve una respuesta HTTP con un código de estado 404 (Not Found)
            }
            // Validar que el nombre no esté duplicado (si cambió)
            if (!string.Equals(ingrediente.Nombre, ingredienteModificacionDto.Nombre, StringComparison.OrdinalIgnoreCase))//aqui se verifica si el nombre del ingrediente ha cambiado y si el nuevo nombre ya existe en la base de datos
            {
                var existe = await context.Ingredientes
                    .AnyAsync(i => i.Nombre.ToLower() == ingredienteModificacionDto.Nombre.ToLower() && i.Id != id);//aqui se verifica si ya existe un ingrediente con el nuevo nombre en la base de datos
                if (existe)
                {
                    return BadRequest(new { mensaje = "Ya existe un ingrediente con ese nombre" });//si ya existe un ingrediente con el nuevo nombre, se devuelve una respuesta HTTP con un código de estado 400 (Bad Request)
                }
            }
            mapper.Map(ingredienteModificacionDto, ingrediente);//aqui se mapea los datos de modificación del DTO a la entidad de ingrediente existente
            context.Ingredientes.Update(ingrediente);//aqui se actualiza el ingrediente en la base de datos
            await context.SaveChangesAsync();//aqui se guardan los cambios en la base de datos

            return Ok(new { mensaje = "Ingrediente actualizado correctamente", data = mapper.Map<IngredienteDto>(ingrediente) });
        }

        //DELETE: api/ingredientes/{id} es para eliminar un ingrediente existente
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteIngrediente(int id)
        {
            var ingrediente = await context.Ingredientes.FirstOrDefaultAsync(i => i.Id == id);//aqui se accede a la base de datos para obtener el ingrediente con el ID especificado
            if (ingrediente == null)//aqui se verifica si el ingrediente no fue encontrado en la base de datos
            {
                return NotFound(new { mensaje = "Ingrediente no encontrado" });//si el ingrediente no fue encontrado, se devuelve una respuesta HTTP con un código de estado 404 (Not Found)
            }

            context.Ingredientes.Remove(ingrediente);//aqui se elimina el ingrediente de la base de datos
            await context.SaveChangesAsync();//aqui se guardan los cambios en la base de datos
            return Ok(new { mensaje = "Ingrediente eliminado correctamente" });
        }


        }//fin de la clase IngredientesController
}
