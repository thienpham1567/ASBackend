using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Backend.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/{controller}")]
    [EnableCors("MyCors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAppointmentRepository _appointmentRepository;
        public AppointmentController(IAppointmentRepository appointmentRepository, AppDbContext context)
        {
            _context = context;
            _appointmentRepository = appointmentRepository;
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_appointmentRepository.GetAll());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("{id}")]
       [Authorize(Roles = "User,Admin")]
        public IActionResult GetById([FromQuery] int id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment != null)
            {
                return Ok(appointment);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult CreateNew([FromBody] AppointmentModel model)
        {
            try
            {
                var appointment = _appointmentRepository.Create(model);
                return StatusCode(StatusCodes.Status201Created, appointment);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, model);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User")]
        public IActionResult UpdateByID(int id, [FromBody] AppointmentModel model)
        {
            try
            {
                if (_appointmentRepository.Update(id, model))
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User")]
        public IActionResult DeleteAppointmentById(int id)
        {
            try
            {
                var appointment = _appointmentRepository.Delete(id);
                if (appointment != null)
                {
                    return StatusCode(StatusCodes.Status200OK, appointment);
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }
    }
}