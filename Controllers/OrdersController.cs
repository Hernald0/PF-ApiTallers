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

        /* GET: OrdersController/Create
        public ActionResult Create()
        {
            return Ok();
        }

        // POST: OrdersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }

        // GET: OrdersController/Edit/5
        public ActionResult Edit(int id)
        {
            return Ok();
        }

        // POST: OrdersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }

        // GET: OrdersController/Delete/5
        public ActionResult Delete(int id)
        {
            return Ok();
        }

        // POST: OrdersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }*/
    }
}
