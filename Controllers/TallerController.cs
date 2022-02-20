using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UTNApiTalleres.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TallerController : ControllerBase
    {
        private readonly ITallerDao _tallerDao;

        public TallerController(ITallerDao TallerDao)
        {
            _tallerDao = TallerDao;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTaller()
        {
            try
            {
                var talleres = await _tallerDao.findAll();
                return Ok(talleres);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
            
        }


        // GET api/<TallerController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaller(int id)
        {
            //return Ok( _personaDao.find(id));
            try
            {
                var taller = await _tallerDao.find(id);
                if (taller == null)
                    return NotFound();
                return Ok(taller);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaller([FromBody] Taller Taller)
        {
            if (Taller == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _tallerDao.create(Taller);

            return Created("Se creo exitosamente.", oCreated);
        }

        // PUT api/<TallerController>/5
        [HttpPut]
        public async Task<IActionResult> UpdateTaller([FromBody] Taller Taller)
        {

            try
            {
                //validamos que exista la persona
                var taller = await _tallerDao.find(Taller.Id);

                //en caso de no existir retornamos Not Found
                if (taller == null)
                    return NotFound("El taller no existe.");

                //En caso de existir avanzamos con la actualización
                var regActualizados = await _tallerDao.update(Taller);

                if (regActualizados)
                    return Content("Actualización exitosa");
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        // DELETE api/<TallerController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaller(int id)
        {
            try
            {

                var persona = await _tallerDao.find(id);

                if (persona == null)
                    return NotFound("Taller no encontrado.");

                await _tallerDao.delete(id);

                return NoContent();

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}
