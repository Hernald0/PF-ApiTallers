using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaDao _ventaDao;

        public VentasController(IVentaDao VentaDao)
        {
            _ventaDao = VentaDao;
        }

        [HttpGet]
        public async Task<IActionResult> getVentasAll()
        {


            try
            {
                var ventas = await _ventaDao.ObtenerTodas();

                return Ok(ventas);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
             
        }

        [HttpGet("getVenta/{id}")]
        public async  Task<IActionResult> Get(int id)
        {
            var venta = await _ventaDao.ObtenerPorId(id);
            if (venta == null)
                return NotFound();

            return Ok(venta);
        }

        [HttpPost]
        public ActionResult AddVenta([FromBody] VentaCreateDTO venta)
        {
            var nuevaVenta = new VentaCreateDTO
            {
                FechaEmision = venta.FechaEmision,
                Usuario = venta.Usuario,
                ClienteId = venta.ClienteId,
                Items = venta.Items,             
                Observaciones = venta.Observaciones,
                Descuento = venta.Descuento,
                Efectivo = venta.Efectivo,
                MontoTotal = venta.MontoTotal,
                TarjetaCredito = venta.TarjetaCredito,
                MontoTarjetaCredito = venta.MontoTarjetaCredito,
                CuentaCorriente = venta.CuentaCorriente,
    

                // completar otros campos
            };

            _ventaDao.AgregarVenta(venta);
            return CreatedAtAction(nameof(Get), new { id = venta.Id }, venta);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] VentaCreateDTO venta)
        {
            if (id != venta.Id)
                return BadRequest();

            _ventaDao.ModificarVenta(venta);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _ventaDao.DeleteVenta(id);
            return NoContent();
        }


    }
}
