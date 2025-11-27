using Microsoft.AspNetCore.Authentication.JwtBearer; // Para autenticación con JWT (Bearer Token)
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;                // Para manejar claves y firmas de los tokens
using System.Text;
using ApiChallengeFit.Data;
using ApiChallengeFit.Repository;
using ApiChallengeFit.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// CONFIGURACIÓN DE BASE DE DATOS
// ----------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"), // Obtiene la cadena de conexión desde appsettings.json
        new MySqlServerVersion(new Version(8, 0, 40))                    // Indica la versión del servidor MySQL (8.0.40)
    )
);

// Agrega el soporte para controladores (permite usar clases con [ApiController])
builder.Services.AddControllers();

// ----------------------
// CONFIGURACIÓN DEL TOKEN JWT
// ----------------------

// Obtiene la clave secreta desde appsettings.json
var key = builder.Configuration["TokenAuthentication:SecretKey"];
// Convierte la clave secreta a bytes (necesario para crear la clave simétrica)
var keyBytes = Encoding.UTF8.GetBytes(key);

// Configura la autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Parámetros de validación del token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,  // Valida quién emitió el token
            ValidateAudience = true, // Valida quién puede usar el token
            ValidateLifetime = true, // Verifica que no esté expirado
            ValidateIssuerSigningKey = true, // Verifica la firma del token
            ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],     // Valor configurado en appsettings.json
            ValidAudience = builder.Configuration["TokenAuthentication:Audience"], // Valor configurado en appsettings.json
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)                  // Clave secreta para verificar la firma
        };
    });

// Agrega Swagger/OpenAPI (documentación automática de la API)
builder.Services.AddOpenApi();

// ----------------------
// INYECCIÓN DE DEPENDENCIAS (Repositorios)
// ----------------------

// Cada repositorio se registra para poder ser inyectado en los controladores
builder.Services.AddScoped<IRepositoryUsuario, RepositoryUsuario>();

var app = builder.Build();

// Si estás en entorno de desarrollo, activa Swagger para probar la API desde el navegador
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Habilita servir archivos estáticos desde wwwroot
app.UseStaticFiles();

// Redirige automáticamente todas las solicitudes HTTP a HTTPS
app.UseHttpsRedirection();

// Activa la autenticación (revisa si hay un token válido en cada petición)
app.UseAuthentication();

// Activa la autorización (verifica permisos según el token)
app.UseAuthorization();

// Mapea los controladores para que respondan a las rutas definidas
app.MapControllers();

// Inicia la aplicación (empieza a escuchar peticiones HTTP)
app.Run();

