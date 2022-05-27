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
    public class EstadoCivilController : ControllerBase
    {

        private readonly IEstadoCivilDao _EstadoCivilDao;

        public EstadoCivilController(IEstadoCivilDao EstadoCivilDao)
        {
            _EstadoCivilDao = EstadoCivilDao;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllEstadosCiviles()
        {
            try
            {
                var estadosciviles = await _EstadoCivilDao.findAll();

                return Ok(estadosciviles);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        // GET api/<ValuesController>/5

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEstadoCivil(int id)
        {
            try
            {
                var EstadoCivil = await _EstadoCivilDao.find(id);
                
                if (EstadoCivil == null)
                    return NotFound();
                
                return Ok(EstadoCivil);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<IActionResult> CreateEstadoCivil([FromBody] EstadoCivil EstadoCivil)
        {
            if (EstadoCivil == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _EstadoCivilDao.create(EstadoCivil);

            return Created("Se creo exitosamente.", oCreated);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEstadoCivil([FromBody] EstadoCivil EstadoCivil)
        {

            try
            {
                //validamos que exista la persona
                var oEstadoCivil = await _EstadoCivilDao.find(EstadoCivil.Id);

                //en caso de no existir retornamos Not Found
                if (oEstadoCivil == null)
                    return NotFound("El Estado Civil no existe.");

                //En caso de existir avanzamos con la actualización
                var actualizado = await _EstadoCivilDao.update(EstadoCivil);

                if (actualizado)
                    return Content("Actualización exitosa.");
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstadoCivil(int id)
        {

            try
            {

                var EstadoCivil = await _EstadoCivilDao.find(id);

                if (EstadoCivil == null)
                    return NotFound("Estado Civil no encontrado.");

                await _EstadoCivilDao.delete(id);

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
