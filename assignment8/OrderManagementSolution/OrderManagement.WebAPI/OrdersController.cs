using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Domain;
using OrderManagement.Service;

namespace OrderManagement.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;

        public OrdersController(OrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get()
        {
            return _service.SortOrders();
        }

        [HttpGet("{id}")]
        public ActionResult<Order> Get(int id)
        {
            var order = _service.QueryOrders(o => o.Id == id).FirstOrDefault();
            if (order == null)
            {
                return NotFound();
            }
            return order;
        }

        [HttpPost]
        public ActionResult<Order> Post(Order order)
        {
            try
            {
                _service.AddOrder(order);
                return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }
            try
            {
                _service.UpdateOrder(order);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _service.DeleteOrder(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}    