using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;

namespace OnlineBookStore.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private CartService _cartService;

        public CartViewModel CartViewModel { get; set; } = new();
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

        /// <summary>
        /// �Ƴ����ﳵ�������
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostRemoveItemAsync(int orderItemNumber)
        {
            var res = await _cartService.RemoveUserCartSingleItemAsync(orderItemNumber);
            if (res.IsSuccess == true)
            {
                return RedirectToPage("/Cart/Index");
            }
            else
            {
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "�Ƴ����ﳵ����ʧ��, ����ϵ��վ����Ա",
                    RedirectUrl = "/Cart/Index",
                    Seconds = 5
                });
            }
        }

        /// <summary>
        /// ��չ��ﳵ����
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostClearCartAsync()
        {
            var res = await _cartService.ClearUserCartAsync();
            if (res.IsSuccess == true)
            {
                return RedirectToPage("/Cart/Index");
            }
            else
            {
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "�Ƴ����ﳵ����ʧ��, ����ϵ��վ����Ա",
                    RedirectUrl = "/Cart/Index",
                    Seconds = 5
                });
            }
        }
    }
}
