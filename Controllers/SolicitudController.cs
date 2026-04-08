using System.Security.Claims;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiChallengeFit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SolicitudController : ControllerBase
    {
        private readonly IRepositorySolicitud repoSolicitud;

        public SolicitudController(IRepositorySolicitud repo)
        {
            repoSolicitud = repo;
        }

        // GET /api/Solicitud/buscar-entrenadores?nombre=xxx
        // Solo Alumno: busca entrenadores por nombre y devuelve con especialidades
        [HttpGet("buscar-entrenadores")]
        public IActionResult BuscarEntrenadores([FromQuery] string nombre)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden buscar entrenadores." });

            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest(new { mensaje = "Debe ingresar un nombre para buscar." });

            var entrenadores = repoSolicitud.BuscarEntrenadores(nombre);
            return Ok(entrenadores);
        }

        // POST /api/Solicitud
        // Solo Alumno: envía una solicitud de vinculación a un entrenador
        [HttpPost]
        public IActionResult CrearSolicitud([FromBody] SolicitudCrearDto dto)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden enviar solicitudes." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));

            // Verificar si ya está vinculado a ese entrenador
            if (repoSolicitud.EsAlumnoDe(idAlumno, dto.IdEntrenador))
                return BadRequest(new { mensaje = "Ya estás vinculado a este entrenador." });

            // Verificar si ya tiene una solicitud pendiente
            if (repoSolicitud.ExisteSolicitudPendiente(idAlumno, dto.IdEntrenador))
                return BadRequest(new { mensaje = "Ya tenés una solicitud pendiente con este entrenador." });

            var solicitud = new SolicitudVinculacion
            {
                IdAlumno = idAlumno,
                IdEntrenador = dto.IdEntrenador,
                Estado = "Pendiente",
                FechaSolicitud = DateTime.Now
            };

            var res = repoSolicitud.CrearSolicitud(solicitud);

            if (res <= 0)
                return StatusCode(500, "No se pudo crear la solicitud.");

            return Ok(new { mensaje = "Solicitud enviada correctamente.", solicitud = new { solicitud.Id, solicitud.IdEntrenador, solicitud.Estado, solicitud.FechaSolicitud } });
        }

        // GET /api/Solicitud/mis-solicitudes
        // Solo Alumno: ve las solicitudes que envió y su estado
        [HttpGet("mis-solicitudes")]
        public IActionResult ObtenerMisSolicitudes()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden ver sus solicitudes." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));
            var solicitudes = repoSolicitud.ObtenerPorAlumno(idAlumno);

            return Ok(solicitudes);
        }

        // GET /api/Solicitud/pendientes
        // Solo Entrenador: ve las solicitudes pendientes que recibió
        [HttpGet("pendientes")]
        public IActionResult ObtenerPendientes()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden ver solicitudes pendientes." });

            var idEntrenador = int.Parse(User.FindFirstValue("Id"));
            var solicitudes = repoSolicitud.ObtenerPendientesPorEntrenador(idEntrenador);

            return Ok(solicitudes);
        }

        // PUT /api/Solicitud/{id}/aceptar
        // Solo Entrenador: acepta la solicitud y vincula al alumno
        [HttpPut("{id}/aceptar")]
        public IActionResult AceptarSolicitud(int id)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden aceptar solicitudes." });

            var idEntrenador = int.Parse(User.FindFirstValue("Id"));
            var solicitud = repoSolicitud.ObtenerPorId(id);

            if (solicitud == null)
                return NotFound(new { mensaje = "No se encontró la solicitud." });

            if (solicitud.IdEntrenador != idEntrenador)
                return StatusCode(403, new { mensaje = "No tenés permiso para responder esta solicitud." });

            if (solicitud.Estado != "Pendiente")
                return BadRequest(new { mensaje = $"Esta solicitud ya fue {solicitud.Estado.ToLower()}." });

            var res = repoSolicitud.AceptarSolicitud(solicitud);

            if (res <= 0)
                return StatusCode(500, "No se pudo aceptar la solicitud.");

            return Ok(new
            {
                mensaje = $"Solicitud aceptada. {solicitud.Alumno?.Nombre} ahora es tu alumno.",
                solicitud = new { solicitud.Id, solicitud.Estado, solicitud.FechaRespuesta }
            });
        }

        // PUT /api/Solicitud/{id}/rechazar
        // Solo Entrenador: rechaza la solicitud
        [HttpPut("{id}/rechazar")]
        public IActionResult RechazarSolicitud(int id)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden rechazar solicitudes." });

            var idEntrenador = int.Parse(User.FindFirstValue("Id"));
            var solicitud = repoSolicitud.ObtenerPorId(id);

            if (solicitud == null)
                return NotFound(new { mensaje = "No se encontró la solicitud." });

            if (solicitud.IdEntrenador != idEntrenador)
                return StatusCode(403, new { mensaje = "No tenés permiso para responder esta solicitud." });

            if (solicitud.Estado != "Pendiente")
                return BadRequest(new { mensaje = $"Esta solicitud ya fue {solicitud.Estado.ToLower()}." });

            var res = repoSolicitud.RechazarSolicitud(solicitud);

            if (res <= 0)
                return StatusCode(500, "No se pudo rechazar la solicitud.");

            return Ok(new
            {
                mensaje = "Solicitud rechazada.",
                solicitud = new { solicitud.Id, solicitud.Estado, solicitud.FechaRespuesta }
            });
        }
    }

    // DTO para crear solicitud (evita enviar todo el modelo)
    public class SolicitudCrearDto
    {
        public int IdEntrenador { get; set; }
    }
}
