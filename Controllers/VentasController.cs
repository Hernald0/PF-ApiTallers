using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

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

        [HttpPost("postVentaOrden")]
        public ActionResult postVentaOrden([FromBody] OrdenDTO orden)
        {

            // Venta venta = (Venta) await _ventaDao.AgregarVentaOrden(orden);
            //return CreatedAtAction(nameof(Get), new { id = venta.Id }, venta);
            return null;
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

        [HttpPut("modificarVenta")]
        public ActionResult Put([FromBody] VentaCreateDTO venta)
        {
            if (venta.Id == null || venta.Id <= 0)
                return BadRequest();

            int? NroVenta = _ventaDao.ModificarVenta(venta);
            return Ok(new { NroVenta = NroVenta }
            );
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _ventaDao.DeleteVenta(id);
            return NoContent();
        }

        [HttpPut("cancelarVenta/{id}")]
        public ActionResult CancelarVenta(int id)
        {

            try
            {

                var resp = _ventaDao.CancelarVenta(id); ;

                if (resp == 0)
                    return NotFound(new { message = "La venta/presupuesto no fue actualizado." });
                else
                    return Ok(new { message = "Quedó cancelado la venta/presupuesto correctamente" });

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }


    }
}
