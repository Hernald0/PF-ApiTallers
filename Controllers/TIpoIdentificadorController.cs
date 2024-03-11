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
    public class TipoIdentificadorController : ControllerBase
    {

        private readonly ITipoidentificadorDao _tipoidentificadorDao;

        public TipoIdentificadorController(ITipoidentificadorDao tipoidentificadorDao)
        {
            _tipoidentificadorDao = tipoidentificadorDao;
        }

        // GET: api/<TIpoIdentificadorController>
        [HttpGet]
        public async Task<IActionResult> GetAllTipoIdentificadores()
        {
            try
            {
                var tipoidentificadores = await _tipoidentificadorDao.findAll();

                return Ok(tipoidentificadores);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        // GET api/<TIpoIdentificadorController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoidentificador(int id)
        {
            try
            {
                var tipoidentificador = await _tipoidentificadorDao.find(id);

                if (tipoidentificador == null)
                    return NotFound();

                return Ok(tipoidentificador);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<TIpoIdentificadorController>
        [HttpPost]
        public async Task<IActionResult> CreateTipoIdentificador([FromBody] TipoIdentificador tipoidentificador)
        {
            if (tipoidentificador == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _tipoidentificadorDao.create(tipoidentificador);

            return Created("Se creo exitosamente.", oCreated);
        }

        // PUT api/<TIpoIdentificadorController>/5
        [HttpPut]
        public async Task<IActionResult> UpdateEstadoCivil([FromBody] TipoIdentificador tipoidentificador)
        {

            try
            {
                //validamos que exista la persona
                var oTipoIdentificador = await _tipoidentificadorDao.find(tipoidentificador.Id);

                //en caso de no existir retornamos Not Found
                if (oTipoIdentificador == null)
                    return NotFound("El Tipo de Identificador no existe.");

                //En caso de existir avanzamos con la actualización
                var actualizado = await _tipoidentificadorDao.update(tipoidentificador);

                if (actualizado)
                    return Ok(new { response = "Actualización exitosa." });
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<TIpoIdentificadorController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoIdentificador(int id)
        {

            try
            {

                var tipoidentificador = await _tipoidentificadorDao.find(id);

                if (tipoidentificador == null)
                    return NotFound("Estado Civil no encontrado.");

                await _tipoidentificadorDao.delete(id);

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
