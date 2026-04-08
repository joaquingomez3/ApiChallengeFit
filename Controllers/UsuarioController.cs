using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiChallengeFit.Data;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiChallengeFit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IRepositoryUsuario repoUsuario;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _contexto;

        public UsuarioController(IRepositoryUsuario repo, IConfiguration config, IWebHostEnvironment env, AppDbContext contexto)
        {
            repoUsuario = repo;
            _config = config;
            _environment = env;
            _contexto = contexto;
        }


        //metodo login
        [HttpPost("login")] // Ruta POST /api/Usuario/login
        public IActionResult Login([FromForm] string mail, [FromForm] string clave)
        {
            // Buscar el usuario por email
            var usuarioEncontrado = repoUsuario.ObtenerPorEmail(mail);

            // Si el usuario no existe, devuelve error 400
            if (usuarioEncontrado == null)
                return BadRequest("Usuario o contraseña incorrectos");

            // Hashea y compara la contraseña ingresada con la almacenada
            var hash = new PasswordHasher<Usuario>();
            var res = hash.VerifyHashedPassword(usuarioEncontrado, usuarioEncontrado.ClaveHash, clave);

            // Si la contraseña no coincide, devuelve error 400
            if (res == PasswordVerificationResult.Failed)
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
        public IActionResult CrearUsuario([FromBody] CrearUsuarioRequest modelo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Valido que el rol sea válido
            if (modelo.Rol != "Alumno" && modelo.Rol != "Entrenador")
                return BadRequest("El rol debe ser 'Alumno' o 'Entrenador'.");

            // Verifico si ya existe un usuario con ese email
            var existe = repoUsuario.ObtenerPorEmail(modelo.Email);

            if (existe != null)
                return BadRequest("Ya existe un usuario con ese email.");

            // Validar objetivos/especialidades ANTES de crear el usuario
            if (modelo.Rol == "Alumno" && modelo.ObjetivoIds != null && modelo.ObjetivoIds.Count > 0)
            {
                foreach (var idObjetivo in modelo.ObjetivoIds)
                {
                    var objetivoExiste = _contexto.Objetivos.Find(idObjetivo);
                    if (objetivoExiste == null)
                        return BadRequest($"El objetivo con Id {idObjetivo} no existe.");
                }
            }

            if (modelo.Rol == "Entrenador" && modelo.EspecialidadIds != null && modelo.EspecialidadIds.Count > 0)
            {
                foreach (var idEspecialidad in modelo.EspecialidadIds)
                {
                    var especialidadExiste = _contexto.Especialidades.Find(idEspecialidad);
                    if (especialidadExiste == null)
                        return BadRequest($"La especialidad con Id {idEspecialidad} no existe.");
                }
            }

            // Uso transacción para que todo sea atómico
            using var transaction = _contexto.Database.BeginTransaction();
            try
            {
                // Creo usuario nuevo
                var usuario = new Usuario
                {
                    Nombre = modelo.Nombre,
                    Email = modelo.Email,
                    Rol = modelo.Rol,
                    Objetivo = modelo.Rol == "Alumno" ? modelo.Objetivo : null
                };

                // Genero hash de contraseña
                var hasher = new PasswordHasher<Usuario>();
                usuario.ClaveHash = hasher.HashPassword(usuario, modelo.Clave);

                // Guardo usuario en BD para obtener el Id
                _contexto.Usuarios.Add(usuario);
                _contexto.SaveChanges();

                // Si es Alumno y envió objetivos, los inserto en la tabla pivote
                if (modelo.Rol == "Alumno" && modelo.ObjetivoIds != null && modelo.ObjetivoIds.Count > 0)
                {
                    foreach (var idObjetivo in modelo.ObjetivoIds)
                    {
                        _contexto.ObjetivoAlumnos.Add(new ObjetivoAlumno
                        {
                            IdUsuario = usuario.Id,
                            IdObjetivo = idObjetivo
                        });
                    }
                    _contexto.SaveChanges();
                }

                // Si es Entrenador y envió especialidades, las inserto en la tabla pivote
                if (modelo.Rol == "Entrenador" && modelo.EspecialidadIds != null && modelo.EspecialidadIds.Count > 0)
                {
                    foreach (var idEspecialidad in modelo.EspecialidadIds)
                    {
                        _contexto.EspecialidadEntrenadores.Add(new EspecialidadEntrenador
                        {
                            IdUsuario = usuario.Id,
                            IdEspecialidad = idEspecialidad
                        });
                    }
                    _contexto.SaveChanges();
                }

                transaction.Commit();
                return Ok("Usuario creado correctamente.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(500, $"Error al crear el usuario: {ex.Message}");
            }
        }

        // GET /api/Usuario/alumnos/progreso
        // Devuelve los alumnos del entrenador logueado con el progreso de sus rutinas asignadas
        [HttpGet("alumnos/progreso")]
        [Authorize]
        public IActionResult ObtenerAlumnosConProgreso()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden ver sus alumnos." });

            var idEntrenador = int.Parse(User.FindFirstValue("Id"));

            // Obtener alumnos asignados a este entrenador
            var alumnos = repoUsuario.ObtenerAlumnosConProgreso(idEntrenador);

            // Para cada alumno, obtener sus rutinas asignadas con progreso
            var resultado = alumnos.Select(alumno => new
            {
                alumno.Id,
                alumno.Nombre,
                alumno.Email,
                alumno.Objetivo,
                Rutinas = _contexto.UsuarioRutinas
                    .Where(ur => ur.IdUsuario == alumno.Id)
                    .Include(ur => ur.Rutina)
                    .Select(ur => new
                    {
                        ur.Id,
                        ur.IdRutina,
                        NombreRutina = ur.Rutina != null ? ur.Rutina.Nombre : null,
                        ur.FechaAsignacion,
                        ur.FechaFinalizacion,
                        ur.Completado
                    })
                    .ToList()
            }).ToList();

            return Ok(resultado);
        }

        // GET /api/Usuario/objetivos?nombre=ganar
        // Devuelve todos los objetivos, o filtra por nombre si se envía el parámetro
        [HttpGet("objetivos")]
        public IActionResult ObtenerObjetivos([FromQuery] string? nombre)
        {
            var query = _contexto.Objetivos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(o => o.Nombre.Contains(nombre));

            var objetivos = query
                .Select(o => new { o.Id, o.Nombre })
                .ToList();

            return Ok(objetivos);
        }

        // GET /api/Usuario/especialidades?nombre=cross
        // Devuelve todas las especialidades, o filtra por nombre si se envía el parámetro
        [HttpGet("especialidades")]
        public IActionResult ObtenerEspecialidades([FromQuery] string? nombre)
        {
            var query = _contexto.Especialidades.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(e => e.Nombre.Contains(nombre));

            var especialidades = query
                .Select(e => new { e.Id, e.Nombre })
                .ToList();

            return Ok(especialidades);
        }
    }
}
