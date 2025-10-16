using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Services;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateNewTaxControlFapiaoApplicationRequest.Types.Fapiao.Types;

namespace OnlineBookStore.Pages.Order
{
    public class CreateModel : PageModel
    {
        private OrderApplication _orderService;

        [BindProperty(SupportsGet = true)]
        public List<OrderItemDto> OrderItems { get; set; } = new();

        [BindProperty]
        public string PaymentMethodString { get; set; } = string.Empty;

        private PaymentMethod _paymentMethod 
        {
            get
            {
                if (PaymentMethodString == "WeChat") 
                    return PaymentMethod.WeChat;

                // 注意啊, 后面记得修
                return PaymentMethod.WeChat;
            }
        }

        public CreateModel(OrderApplication orderService)
        {
            _orderService = orderService;
        }

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync() 
        {
            // 重要：服务端必须再次校验 Items 不为空、数量合法
            if (OrderItems == null || !OrderItems.Any() || string.IsNullOrEmpty(PaymentMethodString)) 
                return BadRequest();

            var result = await _orderService.PlayerOrderAsync(new CreateOrderResponse
            {
                PaymentMethod = _paymentMethod,
                OrderState = OrderState.Finished,
                Items = OrderItems
            });

            if(result.IsSuccessed == true)
            {
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "购买成功",
                    RedirectUrl = "/Index",
                    Seconds = 5
                });
            }
            else
            {
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "购买失败",
                    RedirectUrl = "/Index",
                    Seconds = 5
                });
            }
        }
    }
}
