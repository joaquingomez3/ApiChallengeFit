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
    }
}
