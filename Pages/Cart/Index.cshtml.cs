using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;

namespace OnlineBookStore.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private CartService _cartService;

        public CartViewModel CartViewModel { get; set; }
        public List<CartItemViewModel> CartItemViewModels => CartViewModel.CartItemViewModels;
        public int UserNumber => CartViewModel.UserNumber;
        public int PageIndex { get; set; } = 1;
        public string ErrorMsg { get; set; }

        public IndexModel(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task OnGet()
        {
            var cartResult = await _cartService.GetUserCartAsync(PageIndex);

            if(cartResult.IsSuccess == true)
            {
                CartViewModel = cartResult.CartViewModel!;
            }
            else
            {
                ErrorMsg = cartResult.ErrorMsg;
            }
        }
    }
}
