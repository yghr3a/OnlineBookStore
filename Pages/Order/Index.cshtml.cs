using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;
using System.Runtime.CompilerServices;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.DepositMarketingMemberCardOpenCardCodesResponse.Types;

namespace OnlineBookStore.Pages.Order
{
    [Authorize] // 确保用户已登录
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

        /// <summary>
        /// 重新下单Post请求
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostReplaceOrderAsync(int orderNumber)
        {
            var result = await _orderApplication.ReplaceOrderAsync(orderNumber);

            // 这部分逻辑和Create.cshtml.cs里边的OnPostAsync一模一样
            if (result.IsSuccess == true)
            {
                return RedirectToPage("/Payment/PaymentQr", new
                {
                    OrderNumber = result.Data!.TransactionId,
                    CodeUrl = result.Data!.CodeUrl
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
