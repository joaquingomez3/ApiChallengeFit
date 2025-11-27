using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiChallengeFit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IRepositoryUsuario repoUsuario; // Acceso a los datos de propietarios
        private readonly IConfiguration _config;                   // Acceso a appsettings.json
        private readonly IWebHostEnvironment _environment;          // Para trabajar con archivos y rutas físicas

        // Constructor con inyección de dependencias
        public UsuarioController(IRepositoryUsuario repo, IConfiguration config, IWebHostEnvironment env)
        {
            repoUsuario = repo;
            _config = config;
            _environment = env;
        }


        //metodo login
        [HttpPost("login")] // Ruta POST /api/Usuario/login
        public IActionResult Login([FromForm] string mail, [FromForm] string clave)
        {
            // Buscar el usuario por email
            var usuarioEncontrado = repoUsuario.ObtenerPorEmail(mail);

            // Hashea y compara la contraseña ingresada con la almacenada
            var hash = new PasswordHasher<Usuario>();

            var res = hash.VerifyHashedPassword(usuarioEncontrado, usuarioEncontrado.ClaveHash, clave);

            // Si el usuario no existe o la contraseña no coincide, devuelve error 400
            if (usuarioEncontrado == null || res == PasswordVerificationResult.Failed)
                return BadRequest("Usuario o contraseña incorrectos");

            // Crea los datos (claims) que se incluirán dentro del token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuarioEncontrado.Email), // Guarda el email en el token
                new Claim("Id", usuarioEncontrado.Id.ToString()), // Guarda el ID del propietario
                new Claim(ClaimTypes.Role, usuarioEncontrado.Rol) // Guarda el rol del usuario
            };

            // Obtiene la clave secreta y genera las credenciales de firma
            var secreto = _config["TokenAuthentication:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreto));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crea el token JWT con los claims, expiración y firma
            var token = new JwtSecurityToken(
                issuer: _config["TokenAuthentication:Issuer"],     // Quién emite el token
                audience: _config["TokenAuthentication:Audience"], // Quién puede usarlo
                claims: claims,                                    // Datos que contiene el token
                expires: DateTime.Now.AddHours(1),                 // Duración del token (1 hora)
                signingCredentials: creds                          // Firma digital
            );

            // Devuelve el token generado al cliente
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

         [HttpPost("crear")] // ruta POST /api/Usuario/crear
        public IActionResult CrearUsuario([FromBody] Usuario modelo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifico si ya existe un usuario con ese email
            var existe = repoUsuario.ObtenerPorEmail(modelo.Email);

            if (existe != null)
                return BadRequest("Ya existe un usuario con ese email.");

            // Creo usuario nuevo
            var usuario = new Usuario
            {
                Nombre = modelo.Nombre,
                Email = modelo.Email,
                Rol = modelo.Rol
                
            };

            // Genero hash de contraseña
            var hasher = new PasswordHasher<Usuario>();
            usuario.ClaveHash = hasher.HashPassword(usuario, modelo.ClaveHash);

            // Guardo en BD
            var res = repoUsuario.Alta(usuario);

            if (res <= 0)
                return StatusCode(500, "No se pudo crear el usuario.");

            return Ok("Usuario creado correctamente.");
        }


    }
}
