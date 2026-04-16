using System.Security.Claims;
using ApiChallengeFit.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiChallengeFit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RutinaController : ControllerBase
    {
        private readonly IRepositoryRutina repoRutina;

        public RutinaController(IRepositoryRutina repo)
        {
            repoRutina = repo;
        }

        // GET /api/Rutina
        // Entrenador: devuelve sus rutinas | Alumno: TODO
        [HttpGet]
        public IActionResult ObtenerRutinas()
        {
            // Extraer rol e id del token JWT
            var rol = User.FindFirstValue(ClaimTypes.Role);
            var idStr = User.FindFirstValue("Id");

            if (rol == "Entrenador")
            {
                var idEntrenador = int.Parse(idStr);
                var rutinas = repoRutina.ObtenerPorEntrenador(idEntrenador);
                return Ok(rutinas);
            }
            else if (rol == "Alumno")
            {
                var idAlumno = int.Parse(idStr);
                var rutinas = repoRutina.ObtenerPorAlumno(idAlumno);
                return Ok(rutinas);
            }
            else
            {
                return StatusCode(403, new { mensaje = "Acceso denegado: rol no reconocido." });
            }
        }

        // POST /api/Rutina
        // Solo Entrenador puede crear rutinas; Alumno recibe 403
        [HttpPost]
        public IActionResult CrearRutina([FromBody] Rutina modelo)
        {
            // Extraer rol del token JWT
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden crear rutinas." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Asignar el entrenador desde el token
            var idStr = User.FindFirstValue("Id");
            modelo.IdEntrenador = int.Parse(idStr);

            var res = repoRutina.Alta(modelo);

            if (res <= 0)
                return StatusCode(500, "No se pudo crear la rutina.");

            return Ok(new { mensaje = "Rutina creada correctamente.", rutina = modelo });
        }

        // DELETE /api/Rutina/ejercicio/{id}
        // Elimina un RutinaEjercicio solo si la rutina pertenece al entrenador logueado
        [HttpDelete("ejercicio/{id}")]
        public IActionResult EliminarRutinaEjercicio(int id)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden eliminar ejercicios de rutinas." });

            // Buscar el RutinaEjercicio con su Rutina asociada
            var rutinaEjercicio = repoRutina.ObtenerRutinaEjercicioPorId(id);

            if (rutinaEjercicio == null)
                return NotFound(new { mensaje = "No se encontró el ejercicio de rutina con ese Id." });

            // Verificar que la rutina pertenece al entrenador logueado
            var idEntrenador = int.Parse(User.FindFirstValue("Id"));

            if (rutinaEjercicio.Rutina == null || rutinaEjercicio.Rutina.IdEntrenador != idEntrenador)
                return StatusCode(403, new { mensaje = "No tenés permiso para modificar esta rutina." });

            var res = repoRutina.EliminarRutinaEjercicio(rutinaEjercicio);

            if (res <= 0)
                return StatusCode(500, "No se pudo eliminar el ejercicio de la rutina.");

            return Ok(new { mensaje = "Ejercicio eliminado de la rutina correctamente." });
        }

        // GET /api/Rutina/hoy
        // Solo Alumno: devuelve la rutina del día con ejercicios detallados
        [HttpGet("hoy")]
        public IActionResult ObtenerRutinaDelDia()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden acceder a este recurso." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));
            var rutina = repoRutina.ObtenerRutinaDelDia(idAlumno);

            if (rutina == null)
                return Ok(new { mensaje = "No tenés rutinas pendientes por hoy.", rutina = (object?)null });

            return Ok(new
            {
                rutina.Id,
                rutina.Nombre,
                rutina.Nivel,
                rutina.Descripcion,
                rutina.Duracion,
                Ejercicios = rutina.RutinaEjercicios?.Select(re => new
                {
                    re.Id,
                    NombreEjercicio = re.Ejercicio != null ? re.Ejercicio.Nombre : null,
                    GrupoMuscular = re.Ejercicio != null ? re.Ejercicio.GrupoMuscular : null,
                    re.Series,
                    re.Repeticiones,
                    re.Completado
                })
            });
        }

        // POST /api/Rutina/iniciar/{idRutina}
        // Solo Alumno: registra que el alumno inició una rutina
        [HttpPost("iniciar/{idRutina}")]
        public IActionResult IniciarRutina(int idRutina)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden iniciar rutinas." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));
            var res = repoRutina.IniciarRutina(idAlumno, idRutina);

            if (res == -1)
                return NotFound(new { mensaje = "Esta rutina no está asignada a tu cuenta." });

            if (res == -2)
                return BadRequest(new { mensaje = "Ya tenés esta rutina en progreso." });

            if (res <= 0)
                return StatusCode(500, "No se pudo iniciar la rutina.");

            return Ok(new { mensaje = "Rutina iniciada correctamente." });
        }

        // PUT /api/Rutina/completar/{idRutina}
        // Solo Alumno: marca la rutina como completada
        [HttpPut("completar/{idRutina}")]
        public IActionResult CompletarRutina(int idRutina)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden completar rutinas." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));
            var res = repoRutina.CompletarRutina(idAlumno, idRutina);

            if (res == -1)
                return NotFound(new { mensaje = "No se encontró una rutina pendiente con ese Id." });

            if (res <= 0)
                return StatusCode(500, "No se pudo completar la rutina.");

            return Ok(new { mensaje = "Rutina completada correctamente. ¡Buen trabajo!" });
        }

        // PUT /api/Rutina/ejercicio/{id}/completar
        // Solo Alumno: marca un ejercicio específico dentro de una rutina como completado
        [HttpPut("ejercicio/{id}/completar")]
        public IActionResult CompletarEjercicio(int id)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Alumno")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los alumnos pueden marcar ejercicios como completados." });

            var idAlumno = int.Parse(User.FindFirstValue("Id"));
            var res = repoRutina.CompletarEjercicio(idAlumno, id);

            if (res == -1)
                return NotFound(new { mensaje = "El ejercicio no existe, no está asignado a tu cuenta o ya se completó tu rutina." });

            if (res <= 0)
                return StatusCode(500, "No se pudo completar el ejercicio.");

            return Ok(new { mensaje = "Ejercicio completado" });
        }

        // GET /api/Rutina/buscar-ejercicios?nombre=xxx
        // Solo Entrenador: busca ejercicios por nombre para agregar a la rutina
        [HttpGet("buscar-ejercicios")]
        public IActionResult BuscarEjercicios([FromQuery] string nombre)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden buscar ejercicios." });

            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest(new { mensaje = "Debe ingresar un nombre para buscar." });

            var ejercicios = repoRutina.BuscarEjercicios(nombre);
            
            // Para evitar ciclos y datos extraños mandamos un DTO anónimo si es necesario,
            // pero si la serialización de Ejercicio está bien con el modificador en ICollection, con select alcanza.
            var resultado = ejercicios.Select(e => new
            {
                e.Id,
                e.Nombre,
                e.GrupoMuscular
            });

            return Ok(resultado);
        }

        // POST /api/Rutina/asignar
        // Solo Entrenador: asigna una de sus rutinas a uno de sus alumnos
        [HttpPost("asignar")]
        public IActionResult AsignarRutina([FromBody] AsignarRutinaDto dto)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden asignar rutinas." });

            var idEntrenador = int.Parse(User.FindFirstValue("Id"));

            var res = repoRutina.AsignarRutina(idEntrenador, dto.IdAlumno, dto.IdRutina);

            return res switch
            {
                -1 => NotFound(new { mensaje = "La rutina no existe o no te pertenece." }),
                -2 => BadRequest(new { mensaje = "El alumno no existe o no está vinculado a tu cuenta." }),
                -3 => BadRequest(new { mensaje = "El alumno ya tiene esta rutina asignada." }),
                <= 0 => StatusCode(500, "No se pudo asignar la rutina."),
                _ => Ok(new { mensaje = "Rutina asignada correctamente al alumno." })
            };
        }

        // POST /api/Rutina/{idRutina}/ejercicio
        // Solo Entrenador: agrega un ejercicio a una rutina propia
        [HttpPost("{idRutina}/ejercicio")]
        public IActionResult AgregarEjercicio(int idRutina, [FromBody] AgregarEjercicioDto dto)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden editar rutinas." });

            var idEntrenador = int.Parse(User.FindFirstValue("Id"));

            // Verificar que la rutina existe y pertenece al entrenador
            var rutina = repoRutina.ObtenerRutinaPorId(idRutina);

            if (rutina == null || rutina.IdEntrenador != idEntrenador)
                return NotFound(new { mensaje = "La rutina no existe o no te pertenece." });

            var rutinaEjercicio = new RutinaEjercicio
            {
                IdRutina = idRutina,
                IdEjercicio = dto.IdEjercicio,
                Series = dto.Series,
                Repeticiones = dto.Repeticiones
            };

            var res = repoRutina.AgregarEjercicioARutina(rutinaEjercicio);

            if (res <= 0)
                return StatusCode(500, "No se pudo agregar el ejercicio a la rutina.");

            return Ok(new { mensaje = "Ejercicio agregado a la rutina correctamente.", rutinaEjercicio = new { rutinaEjercicio.Id, rutinaEjercicio.IdRutina, rutinaEjercicio.IdEjercicio, rutinaEjercicio.Series, rutinaEjercicio.Repeticiones } });
        }

        // PUT /api/Rutina/ejercicio/{id}
        // Solo Entrenador: edita series y repeticiones de un ejercicio en una rutina propia
        [HttpPut("ejercicio/{id}")]
        public IActionResult EditarEjercicio(int id, [FromBody] EditarEjercicioDto dto)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (rol != "Entrenador")
                return StatusCode(403, new { mensaje = "Acceso denegado: solo los entrenadores pueden editar rutinas." });

            var idEntrenador = int.Parse(User.FindFirstValue("Id"));

            // Verificar que el RutinaEjercicio existe y la rutina pertenece al entrenador
            var rutinaEjercicio = repoRutina.ObtenerRutinaEjercicioPorId(id);

            if (rutinaEjercicio == null)
                return NotFound(new { mensaje = "No se encontró el ejercicio de rutina con ese Id." });

            if (rutinaEjercicio.Rutina == null || rutinaEjercicio.Rutina.IdEntrenador != idEntrenador)
                return StatusCode(403, new { mensaje = "No tenés permiso para modificar esta rutina." });

            var res = repoRutina.EditarRutinaEjercicio(id, dto.Series, dto.Repeticiones);

            if (res <= 0)
                return StatusCode(500, "No se pudo editar el ejercicio.");

            return Ok(new { mensaje = "Ejercicio actualizado correctamente.", series = dto.Series, repeticiones = dto.Repeticiones });
        }
    }

    // DTO para asignar rutina a un alumno
    public class AsignarRutinaDto
    {
        public int IdAlumno { get; set; }
        public int IdRutina { get; set; }
    }

    // DTO para agregar un ejercicio a una rutina
    public class AgregarEjercicioDto
    {
        public int IdEjercicio { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
    }

    // DTO para editar series/repeticiones de un ejercicio en una rutina
    public class EditarEjercicioDto
    {
        public int Series { get; set; }
        public int Repeticiones { get; set; }
    }
}
