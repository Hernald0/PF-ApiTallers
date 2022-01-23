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
    public class GeneroController : ControllerBase
    {

        private readonly IGeneroDao _generoDao;

        public GeneroController(IGeneroDao generoDao)
        {
            _generoDao = generoDao;
        }

        // GET: api/<GeneroController>
        [HttpGet]
        public async Task<IActionResult> GetAllGenero()
        {
            try
            {
                var generos = await _generoDao.findAll();

                return Ok(generos);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        // GET api/<GeneroController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenero(int id)
        {
            try
            {
                var genero = await _generoDao.find(id);
                if (genero == null)
                    return NotFound();
                return Ok(genero);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<GeneroController>
        [HttpPost]
        public async Task<IActionResult> CreateGenero([FromBody] Genero genero)
        {
            if (genero == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _generoDao.create(genero);

            return Created("Se creo exitosamente.", oCreated);
        }

        // PUT api/<GeneroController>/5
        [HttpPut]
        public async Task<IActionResult> UpdateGenero([FromBody] Genero Genero)
        {

            try
            {
                //validamos que exista la persona
                var oGenero = await _generoDao.find(Genero.Id);

                //en caso de no existir retornamos Not Found
                if (oGenero == null)
                    return NotFound("El género no existe.");

                //En caso de existir avanzamos con la actualización
                var actualizado = await _generoDao.update(Genero);

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

        // DELETE api/<GeneroController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenero(int id)
        {

            try
            {

                var oGenero = await _generoDao.find(id);

                if (oGenero == null)
                    return NotFound("Género no encontrado.");

                await _generoDao.delete(id);

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
