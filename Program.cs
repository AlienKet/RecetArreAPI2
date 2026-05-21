using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecetArreAPI2.Context;
using RecetArreAPI2.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios básicos y AutoMapper
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// --- AQUÍ ACTIVAMOS SWAGGER EN LOS SERVICIOS ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Configurar la seguridad de Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. Conexión a la base de datos (Leyendo tu appsettings de Somee)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (!string.IsNullOrEmpty(connectionString))
    {
        options.UseSqlServer(connectionString);
    }
});

// 4. Configurar JWT
var llaveSecreta = builder.Configuration["LlaveJWT"] ?? "IZbM86D4!LOX%a7z$AXsdvfrrHyBDyhRTUuikX@5B@NL52rRergc54!$%kSÑOKJIAUSFCIK.LQAWIUJJAV5S748D63VC2!";
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

// 5. Configurar Controladores e ignorar ciclos
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// 6. Configurar CORS
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


// A. ACTIVAMOS CORS PRIMERO QUE NADA
app.UseCors("AllowAll");

// B. LEVANTAMOS SWAGGER (Sin importar si es Desarrollo o Producción)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RecetArre API V1");
    options.RoutePrefix = "swagger"; // Esto define que entraremos mediante /swagger
});

// C. Seguridad y Controladores
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();