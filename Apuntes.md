**Ejecutar:**
Abrir PowerShell de windows

cd C:\Users\elneg\source\repos\RecetArreAPI2\RecetArreAPI2

dotnet run --launch-profile https

https://localhost:7019/scalar/v1

**ver errores:**
dotnet build 

**Prueba:**
 "email": "alien1234@gmail.com",
"password": "Alien123!"
Authorization
Bearer tokenGenerado

**Orden para agregar una tabla:**
1-Modelo
2-Context 
3-Migración    
dotnet ef migrations add TRecetas_TComentarios
dotnet ef database update

4-DTOs
5-AutoMapper
6-Controlador 

**Que es cada uno y para que sirve?**

1-Modelo: Es la clase que representa la tabla en la base de datos. 
Define las columnas que va a tener la tabla, por ejemplo `Id`, `Contenido`, `FechaCreacion`, etc.

2-Context: Es el puente entre el código y la base de datos. 
Aquí le dices a Entity Framework que existe la tabla `Comentarios` y cómo está configurada, 
sus restricciones, relaciones con otras tablas, índices, etc.

3-Migración: Es el archivo que genera Entity Framework para crear o modificar la tabla 
en la base de datos real. Es como un historial de cambios que se aplican a la base de datos.

4-DTOs: Son clases que definen qué datos se envían y reciben a través de la API. 
Por ejemplo el `ComentarioDto` es lo que devuelves al cliente, 
el `ComentarioCreacionDto` es lo que recibes cuando alguien quiere crear un comentario. 
Sirven para no exponer directamente el modelo.

5-AutoMapper: Es una librería que copia automáticamente los datos de un objeto a otro, 
por ejemplo de `Comentario` a `ComentarioDto` sin tener que hacerlo manualmente propiedad por propiedad.

6-Controlador:Es donde defines los endpoints de la API, 
es decir las rutas a las que el frontend puede hacer peticiones como 
`GET /api/comentarios`, `POST /api/comentarios`, `DELETE /api/comentarios/{id}`


Ranking: Cual es la mejor receta
Raiting: Puntuacion de la receta en estrellas
