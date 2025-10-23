using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdenDao _orderDao;

        public OrdersController(IOrdenDao orderDao)
        {
            _orderDao = orderDao;
        }


        [HttpGet("ordersAll")]
        // GET: OrdersController
        public async Task<ActionResult> GetOrders()
        {
            try
            {
                var orders = await _orderDao.GetOrders();

                return Ok(orders);

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

      
    }
}
