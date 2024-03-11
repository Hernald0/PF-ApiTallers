using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UTNApiTalleres.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaisController : ControllerBase
    {

        private readonly IPaisDao _paisDao;

        public PaisController(IPaisDao paisdao)
        {
            _paisDao = paisdao;
        }

        // GET: api/<TIpoIdentificadorController>
        [HttpGet]
        public async Task<IActionResult> GetAllPaises()
        {
            try
            {
                var paises = await _paisDao.findAll();

                return Ok(paises);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        // GET api/<TIpoIdentificadorController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPais(int id)
        {
            try
            {
                var oPais = await _paisDao.find(id);

                if (oPais == null)
                    return NotFound();

                return Ok(oPais);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<TIpoIdentificadorController>
        [HttpPost]
        public async Task<IActionResult> CreateTipoIdentificador([FromBody] Pais pais)
        {
            if (pais == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _paisDao.create(pais);

            return Created("Se creo exitosamente.", oCreated);
        }

        // PUT api/<PaisController>/5
        [HttpPut]
        public async Task<IActionResult> UpdatePais([FromBody] Pais pais)
        {

            try
            {
                //validamos que exista 
                var oPais = await _paisDao.find(pais.Id);

                //en caso de no existir retornamos Not Found
                if (oPais == null)
                    return NotFound("El País no existe.");

                //En caso de existir avanzamos con la actualización
                var actualizado = await _paisDao.update(pais);

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

        // DELETE api/<PaisController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePais(int id)
        {

            try
            {

                var pais = await _paisDao.find(id);

                if (pais == null)
                    return NotFound("País no encontrado.");

                await _paisDao.delete(id);

                return NoContent();

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        // GET: api/<TIpoIdentificadorController>
        [HttpGet]
        [Route("Localidades/{cadena}")]
        public async Task<IActionResult> GetAllLocalidades(String cadena)
        {
            try
            {
                var localidades = await _paisDao.findLocalidadesAll(cadena);

                return Ok(localidades);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        // GET: api/<TIpoIdentificadorController>
        /*[HttpGet("{id}")]
        [Route("Provincias/{id}")]*/
        [HttpGet]
        [Route("Provincias/{id}")]
        public async Task<IActionResult> GetAllProvincias(int id)
        {
            try
            {
                var provincias = await _paisDao.findProvinciaAll(id);

                return Ok(provincias);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }
    }
}
