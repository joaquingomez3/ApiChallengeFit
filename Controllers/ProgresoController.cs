using System.Security.Claims;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiChallengeFit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProgresoController : ControllerBase
    {
        private readonly IRepositoryProgreso repoProgreso;

        public ProgresoController(IRepositoryProgreso repo)
        {
            repoProgreso = repo;
        }

        // GET /api/Progreso/semanal
        // Solo Alumno: devuelve el porcentaje de progreso semanal
        [HttpGet("semanal")]
        public IActionResult ObtenerProgresoSemanal()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden acceder a este recurso." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));
            var progreso = repoProgreso.ObtenerProgresoSemanal(idAlumno);

            return Ok(progreso);
        }

        // GET /api/Progreso/general
        // Solo Alumno: devuelve el progreso general (%, rutinas completadas, desafíos terminados)
        [HttpGet("general")]
        public IActionResult ObtenerProgresoGeneral()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden acceder a este recurso." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));
            var progreso = repoProgreso.ObtenerProgresoGeneral(idAlumno);

            return Ok(progreso);
        }

        // GET /api/Progreso/rendimiento
        // Solo Alumno: devuelve los datos de rendimiento de los últimos 7 días
        [HttpGet("rendimiento")]
        public IActionResult ObtenerRendimiento()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden acceder a este recurso." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));
            var rendimiento = repoProgreso.ObtenerRendimiento(idAlumno);

            return Ok(rendimiento);
        }

        // GET /api/Progreso/alumno/{idAlumno}/rutinas?estado=activas
        // Solo Entrenador: devuelve las rutinas asigandas a un alumno con el porcentaje de completitud
        [HttpGet("alumno/{idAlumno}/rutinas")]
        public IActionResult ObtenerRutinasDeAlumno(int idAlumno, [FromQuery] string estado = "activas")
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden acceder a las rutinas de sus alumnos." });

            var idEntrenadorStr = User.FindFirstValue("Id");
            if (!int.TryParse(idEntrenadorStr, out int idEntrenador))
            {
                 return Unauthorized(new { mensaje = "Token inválido." });
            }

            var rutinas = repoProgreso.ObtenerRutinasConProgresoPorAlumno(idEntrenador, idAlumno, estado);

            if (rutinas == null)
            {
                return StatusCode(403, new { mensaje = "Acceso denegado: el alumno no existe o no está vinculado a tu cuenta." });
            }

            return Ok(rutinas);
        }
    }
}
