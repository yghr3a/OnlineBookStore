using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Services;

namespace OnlineBookStore.Controllers
{
    // Controllers/PaymentNotifyController.cs
    // 用于处理第三方支付平台的支付通知
    [ApiController]
    [Route("api/payment")]
    public class PaymentNotifyController : ControllerBase
    {
        private readonly OrderApplication _orderAppService;

        public PaymentNotifyController(OrderApplication orderAppService)
        {
            _orderAppService = orderAppService;
        }

        [HttpPost("notify")]
        public async Task<IActionResult> Notify([FromBody] PaymentNotifyModel model)
        {
            if (model.Success)
            {
                // 如果成功就更新订单状态为已支付
                await _orderAppService.UpdateOrderPaymentStatusAsync(model.OrderNumber, OrderStatus.Success);
            }
            else
            {
                await _orderAppService.UpdateOrderPaymentStatusAsync(model.OrderNumber, OrderStatus.Fail);
            }
            // 之后支付CheckOut页面会轮询订单状态, 显示支付结果

            // 返回 200 表示已处理（模拟第三方要求）
            return Ok(new { result = "received" });
        }
    }

}
