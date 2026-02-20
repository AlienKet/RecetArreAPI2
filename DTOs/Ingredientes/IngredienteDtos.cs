namespace RecetArreAPI2.DTOs.Ingredientes
{

    //DTO es una clase que se utiliza para transferir datos entre capas de una aplicación,
    //en este caso entre el controlador y la vista, o entre el controlador y el servicio.
    //Se utiliza para evitar exponer directamente las entidades del modelo de datos,
    //y para adaptar los datos a las necesidades específicas de la vista o del servicio. 
    public class IngredienteDto//DTO para mostrar los ingredientes
    {

        public int Id { get; set; }

        public string Nombre { get; set; } = default!;

        public string UnidadMedida { get; set; } = default!;

        public string? Descripcion { get; set; } // ? significa que es opcional
        public DateTime CreadoUtc { get; set; }
    }

    public class IngredienteCreacionDto //DTO para crear ingredientes
    {
        public string Nombre { get; set; } = default!;
        public string UnidadMedida { get; set; } = default!;
        public string? Descripcion { get; set; }
    }

    public class IngredienteModificacionDto //DTO para modificar ingredientes
    {
        public string Nombre { get; set; } = default!;
        public string UnidadMedida { get; set; } = default!;
        public string? Descripcion { get; set; }
    }

}
