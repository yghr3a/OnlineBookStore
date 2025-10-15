using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Services;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Pages.Order
{
    public class CreateOrderIndexModel : PageModel
    {
        private OrderService _orderService;
        public CreateOrderIndexModel(OrderService orderService)
        {
            _orderService = orderService;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost() 
        {
            var result = await _orderService.CreateOrderAsync(new CreateOrderResponse
            {
                PaymentMethod = PaymentMethod.WeChat,
                OrderState = OrderState.Finished,
                Items = new List<CreateOrderItem>()
                {
                    { new CreateOrderItem{ BookNumber = 2002, Count =1 } },
                    { new CreateOrderItem{ BookNumber = 2000, Count =5 } },
                    { new CreateOrderItem{ BookNumber = 3000, Count = 2 } }
                }
            });

            if(result.IsSuccessed == true)
            {
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "¹ºÂò³É¹¦",
                    RedirectUrl = "/Index",
                    Seconds = 5
                });
            }
            else
            {
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "¹ºÂòÊ§°Ü",
                    RedirectUrl = "/Index",
                    Seconds = 5
                });
            }
        }
    }
}
