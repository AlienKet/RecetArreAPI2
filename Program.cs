using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecetArreAPI2.Context;
using RecetArreAPI2.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);// Crear el builder de la aplicación
//builder es un objeto que se utiliza para configurar y construir la aplicación web

// Servicios básicos y AutoMapper
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Activar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configurar la seguridad de Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //si la cadena de conexión no es nula o vacía,
    //se configura el contexto de la base de datos para usar SQL Server con esa cadena de conexión
    if (!string.IsNullOrEmpty(connectionString)) 
    {
        options.UseSqlServer(connectionString);
    }
});

//Configurar JWT
var llaveSecreta = builder.Configuration["LlaveJWT"] ?? "IZbM86D4!LOX%a7z$AXsdvfrrHyBDyhRTUuikX@5B@NL52rRergc54!$%kSÑOKJIAUSFCIK.LQAWIUJJAV5S748D63VC2!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//aqui se especifica que se va a usar JWT para la autenticación
    .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,// No se validará el emisor del token
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(llaveSecreta)),
        ClockSkew = TimeSpan.Zero
    });

// Configurar Controladores e ignorar ciclos
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Activar cors
app.UseCors("AllowAll");

// Levantar Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RecetArre API V1");
    options.RoutePrefix = "swagger"; // Esto define que se entrara mediante /swagger
});

//Seguridad y Controladores
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();