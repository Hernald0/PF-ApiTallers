using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UTNApiTalleres.Application.DTOs;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;
using WebApiTalleres.Models;
using WebApiTalleres.Models.Enum;

namespace UTNApiTalleres.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdenDao _orderDao;
        private readonly IVentaDao _ventaDao;

        public OrdersController(IOrdenDao orderDao, IVentaDao ventaDao)
        {
            _orderDao = orderDao;
            _ventaDao = ventaDao;
        }


        [HttpGet("ordersAll")]
        // GET: OrdersController
        public async Task<ActionResult> GetOrders()
        {
            try
            {
                var rol = User.FindFirst(ClaimTypes.Role)?.Value;
                var idEmpleadoClaim = User.FindFirst("IdEmpleado")?.Value;

                int? idEmpleado = null;

                if (rol != "Jefe de Taller" && rol != "Administrador")
                    idEmpleado = int.Parse(idEmpleadoClaim);

                if (rol ==  "Administrativo")
                    idEmpleado = int.Parse(idEmpleadoClaim);

                var ordenes = await _orderDao.GetOrdenes(rol, idEmpleado);

                //var orders = await _orderDao.GetOrders();

                return Ok(ordenes);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("empleadosMecanicos")]
        // GET: OrdersController
        public async Task<ActionResult> GetEmpleadosMecanicos()
        {
            try
            {
                var empsMecs= await _orderDao.getEmpleadosMecanicos();

                return Ok(empsMecs);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("getOrden/{id}")]
        public async Task<IActionResult> GetOrden(int id)
        {
            Orden oOrden = await _orderDao.GetOrden(id);
            return Ok(oOrden);
        }

        [HttpPost("postOrden")]
        public async Task<IActionResult> postOrden([FromBody] RecepcionTurnoDTO orden)
        {   
            var oOrden = await _orderDao.AgregarOrder(null, orden);
            return Ok(oOrden);
        }

        [HttpPut("modificarEmpleadoAsignado")]
        public async Task<IActionResult> modificarEmpleadoAsignado([FromBody]  EmpleadoAsignadoDTO empleadoAsignado)
        {
            var resp  = await _orderDao.ModificarEmpleadoAsignado(empleadoAsignado);
            return Ok(resp);

        }



        [HttpPut("putOrden")]
        public async Task<IActionResult> modificarOrden([FromBody] OrdenDTO orden)
        {
            _orderDao.ModificarOrder(orden);

            if (orden.Estado == (int?)EstadoOrden.Finalizado)
            {
                int idVenta = await _ventaDao.AgregarVentaOrden(orden);

                _orderDao.UpdateVentaId(orden.IdOrden, idVenta); // o un método específico 
            }
            return Ok();
        }


        [HttpPut("definicionClienteOrden")]
        public async Task<IActionResult> definicionClienterOrden([FromBody] DefinicionClienteOrdenDTO definicion)
        {

            if (definicion.OrderId <= 0 || definicion.Estado <= 0)

                return BadRequest();
           

            await _orderDao.deficionClienteOrder(definicion.OrderId, definicion.Estado);

       
            return Ok();
        }

   


    }
}
