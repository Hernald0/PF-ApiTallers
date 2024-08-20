using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;
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

        [HttpGet("GetClienteByIdentificacion")]
        public async Task<IActionResult> GetClienteByIdentificacion([FromQuery] string IdTipoIdentificador, [FromQuery] string NroIdentificador)
        {
            try
            {
                Console.WriteLine($"Tipo Identificador: {IdTipoIdentificador}, Nro Identificacion: {NroIdentificador}");
                var pvmIdentificador = new vmIdentificador
                                            {
                                                TipoIdentificador = Convert.ToInt32(IdTipoIdentificador),
                                                NroIdentificacion = Convert.ToInt32(NroIdentificador)
                };

                var cliente = await _clienteDao.findByNroIdentificacion(pvmIdentificador);
                //if (cliente == null)
                //    return NotFound();
                return Ok(cliente);
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
        public async Task<IActionResult> UpdateCliente([FromBody] Cliente pCliente)
        {

            try
            {
                //validamos que exista la persona
                //var cliente = await _clienteDao.find(Cliente.Id);

                //en caso de no existir retornamos Not Found
                if (pCliente == null)
                    return NotFound("La Cliente no existe.");

                //En caso de existir avanzamos con la actualización
                var clienteActualizado = await _clienteDao.update(pCliente);

                if (clienteActualizado != null)
                    return Ok(new { clienteActualizado,  response = "Actualización exitosa." });
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("actualizarVehiculo")]
        public async Task<IActionResult> UpdateVehiculo(ClienteVehiculoViewModel payload)
        {
            // Lógica para actualizar el vehículo...
            try
            {

                var cliente = await _clienteDao.UpdateVehiculo(payload.IdCliente, payload.Vehiculo);

                if (cliente == null)
                    return NotFound("Cliente no encontrada.");
                else
                    return Ok(cliente);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("nuevoVehiculo")]
        public async Task<IActionResult> InsertVehiculo(ClienteVehiculoViewModel payload)
        {
            // Lógica para Insertar el vehículo...
            // Lógica para actualizar el vehículo...
            try
            {

                var cliente = await _clienteDao.InsertVehiculo(payload.IdCliente, payload.Vehiculo);

                if (cliente == null)
                    return NotFound("Cliente no encontrada.");
                else
                    return Ok(cliente);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteCliente(int id)
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
        
        
        [HttpDelete("deleteClienteVehiculo/{id}")]
        public async Task<IActionResult> deleteClienteVehiculo(int id)
        {

            try
            {

                var cliente = await _clienteDao.deleteVehiculo(id);

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
