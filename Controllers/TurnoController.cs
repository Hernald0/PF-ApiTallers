using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurnoController : ControllerBase
    {
        private readonly ITurnoDao _turnoDao;

        public TurnoController(ITurnoDao TurnoDao)
        {
            _turnoDao = TurnoDao;
        }

        [HttpGet("turnosAll")]
        public async Task<ActionResult> GetTurnos()
        {
            try
            {
                var turnos =await _turnoDao.GetTurnos();

                return Ok(turnos);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpGet("slots-disponibles")]
        public ActionResult<IEnumerable<TimeSpan>> GetAvailableSlots([FromQuery] string fechaSeleccionada)
        {
            IEnumerable<TimeSpan> slots = _turnoDao.GetDisponibleSlots(fechaSeleccionada);
            
            var formattedHorarios = slots.Select(h => new { Hora = h.ToString(@"hh\:mm") }).ToList();

            return Ok(formattedHorarios);
        }

        [HttpPut("cancelarTurno/{id}")]
        public ActionResult CancelarTurno(int id)
        {

            try
            {

                var resp =  _turnoDao.CancelarTurno(id); ;

                if (resp == 0)
                    return NotFound(new { message = "El turno no fue actualizado." });
                else
                    return Ok(new { message = "Turno actualizado correctamente" });

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("modificarTurno")]
        public ActionResult ModificarTurno([FromBody] Turno turno)
        {

            try
            {

                var resp = _turnoDao.ModificarTurno(turno); ;

                if (resp == 0)
                    return NotFound(new { message = "El turno   fue actualizado." });
                else
                    return Ok(new { message = "Turno actualizado correctamente" });

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddTurno([FromBody] Turno Turno)
        {
                _turnoDao.AgregarTurno(Turno);
           
            return Ok(new {  response = "Actualización exitosa." });
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteTurno(int id)
        {
            _turnoDao.DeleteTurno(id);
            return Ok();
        }
    }
}
