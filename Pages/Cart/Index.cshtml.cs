using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;

namespace OnlineBookStore.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private CartApplication _cartApplication;

        public CartViewModel CartViewModel { get; set; } = new();
        public List<CartItemViewModel> CartItemViewModels => CartViewModel.CartItemViewModels;
        public int UserNumber => CartViewModel.UserNumber;
        public int PageIndex { get; set; } = 1;
        public string ErrorMsg { get; set; }

        public IndexModel(CartApplication cartApplication)
        {
            _cartApplication = cartApplication;
        }

        public async Task OnGet()
        {
            var cartResult = await _cartApplication.GetUserCartAsync(PageIndex);

            if(cartResult.IsSuccess == true)
            {
                CartViewModel = cartResult.Data!;
            }
            else
            {
                ErrorMsg = cartResult.ErrorMsg;
            }
        }

        /// <summary>
        /// 移除购物车单线项方法
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostRemoveItemAsync(int cartItemNumber)
        {
            var res = await _cartApplication.RemoveUserCartSingleItemAsync(cartItemNumber);
            if (res.IsSuccess == true)
            {
                return RedirectToPage("/Cart/Index");
            }
            else
            {
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "移除购物车单项失败, 请联系网站管理员",
                    RedirectUrl = "/Cart/Index",
                    Seconds = 5
                });
            }
        }

        /// <summary>
        /// 晴空购物车方法
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostClearCartAsync()
        {
            var res = await _cartApplication.ClearUserCartAsync();
            if (res.IsSuccess == true)
            {
                return RedirectToPage("/Cart/Index");
            }
            else
            {
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "清理购物车失败, 请联系网站管理员",
                    RedirectUrl = "/Cart/Index",
                    Seconds = 5
                });
            }
        }
    }
}
