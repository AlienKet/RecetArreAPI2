using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecetArreAPI2.Context;
using RecetArreAPI2.Models;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Añadimos OpenAPI y AutoMapper
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// 2. Configurar la seguridad de Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. Conexion a la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. Configurar JWT (Con un salvavidas por si la configuración llega nula a Somee)
var llaveSecreta = builder.Configuration["LlaveJWT"] ?? "ClaveAlternativaSeguraDeMasDe32CaracteresParaEvitarCrash";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(llaveSecreta)),
        ClockSkew = TimeSpan.Zero
    });

// 5. Configuramos los controladores una SOLA vez con sus opciones
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddNewtonsoftJson();

// 6. Configurar Cors
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

// 7. Mapear rutas de documentación
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("RecetArre API")
           .WithTheme(ScalarTheme.Moon)
           .WithOpenApiRoutePattern("/openapi/v1.json");
});

// Dejamos las redirecciones e IFs apagados para Somee
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();