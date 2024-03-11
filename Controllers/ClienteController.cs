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
    public class ClienteController : ControllerBase
    {
        private readonly IClienteDao _clienteDao;

        public ClienteController(IClienteDao clienteDao)
        {
            _clienteDao = clienteDao;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCliente()
        {
            try
            {
                var clientes = await _clienteDao.findAll();

                return Ok(clientes);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCliente(int id)
        {
            try
            {
                var cliente = await _clienteDao.find(id);
                if (cliente == null)
                    return NotFound();
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] Cliente cliente)
        {
            if (cliente == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oCreated = await _clienteDao.create(cliente);

            return Created("Se creo exitosamente.", oCreated);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCliente([FromBody] Cliente Cliente)
        {

            try
            {
                //validamos que exista la persona
                var cliente = await _clienteDao.find(Cliente.Id);

                //en caso de no existir retornamos Not Found
                if (cliente == null)
                    return NotFound("La Cliente no existe.");

                //En caso de existir avanzamos con la actualización
                var actualizado = await _clienteDao.update(Cliente);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {

            try
            {

                var cliente = await _clienteDao.find(id);

                if (cliente == null)
                    return NotFound("Cliente no encontrada.");

                await _clienteDao.delete(id);

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
