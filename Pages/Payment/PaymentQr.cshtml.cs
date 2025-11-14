using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Services;
using OnlineBookStore.Models.Entities;
using QRCoder;

namespace OnlineBookStore.Pages.Payment
{
    public class PaymentQrModel : PageModel
    {
        private MockPaymentGateway _mockQrPayMentService;
        private OrderApplication _orderApplication;

        // 订单编号, 从url作为参数传入
        [BindProperty(SupportsGet = true)] public int OrderNumber { get; set; }

        // 二维码对应的模拟支付URL, 从url作为参数传入
        [BindProperty(SupportsGet = true)] public string CodeUrl { get; set; } = string.Empty;

        public string QrbaseUrl { get; set; } = string.Empty;

        // 支付状态
        public string Status { get; set; } = "等待支付";

        public PaymentQrModel(MockPaymentGateway mockQrPayMentService,
                              OrderApplication orderApplication)
        {
            _mockQrPayMentService = mockQrPayMentService;
            _orderApplication = orderApplication;
        }

        public async Task OnGetAsync()
        {
            // 使用CodeUrl生成二维码链接
            QrbaseUrl = await GenerateQrCodeBase64Async(CodeUrl);
        }

        /// <summary>
        /// 用于轮询状态
        /// </summary>
        /// [2025/11/13] 干脆页面js的判断逻辑也放到这里来写吧, 这样前后端逻辑都在一个地方, 方便维护
        /// 不行...Ajax接受Redirect会报错...老老实实前端返回吧
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetCheckOrderStatus(int OrderNumber)
        {
            // 这里直接查询数据库里的订单状态更合理
            var orderStatusRes = await _orderApplication.CheckOrderStatusByOrderNumber(OrderNumber);

            if (!orderStatusRes.IsSuccess)
                return new JsonResult(new { result = "Error" });

            var orderStatus = orderStatusRes.Data!;

            if (orderStatus == OrderStatus.Success)
                return new JsonResult(new { result = "Success" });
            else if (orderStatus == OrderStatus.Fail)
                return new JsonResult(new { result = "Fail" });
            else
                return new JsonResult(new { result = "Pending" });
        }

        /// <summary>
        /// 生成二维码图片的Base64字符串
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<string> GenerateQrCodeBase64Async(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
        }
    }
}
