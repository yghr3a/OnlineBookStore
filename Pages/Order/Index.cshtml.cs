using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;
using System.Runtime.CompilerServices;

namespace OnlineBookStore.Pages.Order
{
    public class IndexModel : PageModel
    {
        private OrderApplication _orderApplication;

        [BindProperty] public List<OrderViewModel> OrderViewModels { set; get; } = new();
        [BindProperty] public int PageIndex { get; set; } = 1;
        [BindProperty] public string ErrorMsg { get; set; } = string.Empty;

        public IndexModel(OrderApplication orderApplication)
        {
            _orderApplication = orderApplication;
        }
        public async Task OnGet()
        {
            var res = await _orderApplication.GetUserOrderAsync(PageIndex);

            if(res.IsSuccess == true)
            {
                OrderViewModels = res.Data!;
            }
            else
            {
                ErrorMsg = res.ErrorMsg;
            }
        }
    }
}
