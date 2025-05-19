using System;
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

        /// <summary>
        /// 获取所有订单
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetAll()
        {
            return _service.SortOrders();
        }

        /// <summary>
        /// 根据ID获取订单
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Order> GetById(int id)
        {
            var order = _service.QueryOrders(o => o.Id == id).FirstOrDefault();
            if (order == null)
            {
                return NotFound($"未找到ID为{id}的订单");
            }
            return order;
        }

        /// <summary>
        /// 根据客户名称查询订单
        /// </summary>
        [HttpGet("customer/{customerName}")]
        public ActionResult<IEnumerable<Order>> GetByCustomer(string customerName)
        {
            if (string.IsNullOrEmpty(customerName))
                return BadRequest("客户名称不能为空");

            var orders = _service.QueryOrders(o => o.Customer.Contains(customerName));
            if (!orders.Any())
            {
                return NotFound($"未找到客户名称包含 '{customerName}' 的订单");
            }
            return orders;
        }

        /// <summary>
        /// 根据订单号查询订单
        /// </summary>
        [HttpGet("orderNumber/{orderNumber}")]
        public ActionResult<Order> GetByOrderNumber(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
                return BadRequest("订单号不能为空");

            var order = _service.QueryOrders(o => o.OrderNumber == orderNumber).FirstOrDefault();
            if (order == null)
            {
                return NotFound($"未找到订单号为 '{orderNumber}' 的订单");
            }
            return order;
        }

        /// <summary>
        /// 创建新订单
        /// </summary>
        [HttpPost]
        public ActionResult<Order> Create([FromBody] Order order)
        {
            try
            {
                // 基本验证
                if (order == null)
                    return BadRequest("订单数据不能为空");

                if (string.IsNullOrEmpty(order.OrderNumber))
                    return BadRequest("订单号不能为空");

                if (string.IsNullOrEmpty(order.Customer))
                    return BadRequest("客户名称不能为空");

                if (order.OrderDetails == null || !order.OrderDetails.Any())
                    return BadRequest("订单必须包含至少一个商品");

                _service.AddOrder(order);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"添加订单时发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新现有订单
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Order order)
        {
            if (order == null)
                return BadRequest("订单数据不能为空");
                
            if (id != order.Id)
                return BadRequest("订单ID不匹配");

            try
            {
                _service.UpdateOrder(order);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"更新订单时发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除订单
        /// </summary>
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
            catch (Exception ex)
            {
                return StatusCode(500, $"删除订单时发生错误: {ex.Message}");
            }
        }
    }
}    