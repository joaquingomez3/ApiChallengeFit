using System.Security.Claims;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiChallengeFit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DesafioController : ControllerBase
    {
        private readonly IRepositoryDesafio repoDesafio;

        public DesafioController(IRepositoryDesafio repo)
        {
            repoDesafio = repo;
        }

        // GET /api/Desafio
        // Solo Entrenador: devuelve los desafíos que creó
        [HttpGet]
        public IActionResult ObtenerDesafios()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden ver los desafíos." });

            var idEntrenador = int.Parse(User.FindFirstValue("Id"));
            var desafios = repoDesafio.ObtenerPorEntrenador(idEntrenador);

            return Ok(desafios);
        }

        // GET /api/Desafio/mis-desafios
        // Solo Alumno: devuelve los desafíos que le fueron asignados
        [HttpGet("mis-desafios")]
        public IActionResult ObtenerMisDesafios()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden acceder a este recurso." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));
            var desafios = repoDesafio.ObtenerDesafiosConProgreso(idAlumno);

            return Ok(desafios);
        }

        // POST /api/Desafio
        // Solo Entrenador puede crear desafíos; Alumno recibe 403
        [HttpPost]
        public IActionResult CrearDesafio([FromBody] Desafio modelo)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden crear desafíos." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Asignar el entrenador desde el token
            var idStr = User.FindFirstValue("Id");
            modelo.IdEntrenador = int.Parse(idStr);

            var res = repoDesafio.Alta(modelo);

            if (res <= 0)
                return StatusCode(500, "No se pudo crear el desafío.");

            return Ok(new { mensaje = "Desafío creado correctamente.", desafio = modelo });
        }

        // PUT /api/Desafio/progreso/{idDesafioUsuario}
        // Solo Alumno: actualiza el progreso de un desafío asignado
        [HttpPut("progreso/{idDesafioUsuario}")]
        public IActionResult ActualizarProgreso(int idDesafioUsuario, [FromBody] int nuevoProgreso)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden actualizar su progreso." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));

            var desafioUsuario = repoDesafio.ObtenerDesafioUsuarioPorId(idDesafioUsuario);

            if (desafioUsuario == null)
                return NotFound(new { mensaje = "No se encontró el desafío." });

            // Verificar que el desafío pertenece al alumno logueado
            if (desafioUsuario.IdUsuario != idAlumno)
                return StatusCode(403, new { mensaje = "No tenés permiso para modificar este desafío." });

            desafioUsuario.Progreso = nuevoProgreso;

            // Si el progreso llega a 100, marcar como completado
            if (nuevoProgreso >= 100)
                desafioUsuario.Completado = true;

            var res = repoDesafio.ActualizarProgreso(desafioUsuario);

            if (res <= 0)
                return StatusCode(500, "No se pudo actualizar el progreso.");

            return Ok(new
            {
                mensaje = desafioUsuario.Completado
                    ? "¡Desafío completado! ¡Felicitaciones!"
                    : "Progreso actualizado correctamente.",
                progreso = desafioUsuario.Progreso,
                completado = desafioUsuario.Completado
            });
        }

        // POST /api/Desafio/asignar
        // Solo Entrenador: asigna uno de sus desafíos a uno de sus alumnos
        [HttpPost("asignar")]
        public IActionResult AsignarDesafio([FromBody] AsignarDesafioDto dto)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden asignar desafíos." });

            var idEntrenador = int.Parse(User.FindFirstValue("Id"));

            var res = repoDesafio.AsignarDesafio(idEntrenador, dto.IdAlumno, dto.IdDesafio);

            return res switch
            {
                -1 => NotFound(new { mensaje = "El desafío no existe o no te pertenece." }),
                -2 => BadRequest(new { mensaje = "El alumno no existe o no está vinculado a tu cuenta." }),
                -3 => BadRequest(new { mensaje = "El alumno ya tiene este desafío asignado." }),
                <= 0 => StatusCode(500, "No se pudo asignar el desafío."),
                _ => Ok(new { mensaje = "Desafío asignado correctamente al alumno." })
            };
        }
    }

    // DTO para asignar desafío a un alumno
    public class AsignarDesafioDto
    {
        public int IdAlumno { get; set; }
        public int IdDesafio { get; set; }
    }
}
