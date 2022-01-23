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
    public class PersonaController : ControllerBase
    {

        private readonly IPersonaDao _personaDao;

        public PersonaController(IPersonaDao PersonaDao)
        {
            _personaDao = PersonaDao;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPersona()
        {
            try
            {
                var personas = await _personaDao.findAll();
                return Ok(personas);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
            //return Ok(await _personaDao.findAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersona(int id)
        {
            //return Ok( _personaDao.find(id));
            try
            {
                var persona = await _personaDao.find(id);
                if (persona == null)
                    return NotFound();
                return Ok(persona);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersona([FromBody] Persona Persona)
        {
            if (Persona == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _personaDao.create(Persona);

            return Created("Se creo exitosamente.", oCreated);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePersona([FromBody] Persona Persona)
        {  

            try
            {   
                //validamos que exista la persona
                var persona = await _personaDao.find(Persona.Id);
                
                //en caso de no existir retornamos Not Found
                if (persona == null)
                    return NotFound("La persona no existe.");
                
                //En caso de existir avanzamos con la actualización
                var regActualizados = await _personaDao.update(Persona);

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

        [HttpDelete]
        public async Task<IActionResult> DeletePersona(int id)
        {
            try
            {

                var persona = await _personaDao.find(id);
                
                if (persona == null)
                    return NotFound("Persona no encontrada.");
                
                await _personaDao.delete(id);
                
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
