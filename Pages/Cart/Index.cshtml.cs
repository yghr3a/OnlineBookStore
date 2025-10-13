using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;

namespace OnlineBookStore.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private CartService _cartService;

        public CartViewModel cartViewModel { get; set; }
        public string ErrorMsg { get; set; }

        public IndexModel(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task OnGet()
        {
            var cartResult = await _cartService.GetUserCartAsync();

            if(cartResult.IsSuccess == true)
            {
                cartViewModel = cartResult.CartViewModel!;
            }
            else
            {
                ErrorMsg = cartResult.ErrorMsg;
            }
        }
    }
}
