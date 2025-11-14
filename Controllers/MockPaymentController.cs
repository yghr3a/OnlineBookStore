using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Models.Data;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace OnlineBookStore.Controllers
{
    /// <summary>
    /// 模拟第三方支付平台的API控制器
    /// </summary>
    [Route("mockpay")]
    public class MockPaymentController : Controller
    {

        // 模拟第三方支付平台的订单状态存储
        private static readonly ConcurrentDictionary<string, (string OrderId, decimal Amount, string NotifyUrl, string TransactionId)> _store
            = new();

        private static readonly HttpClient _http = new();

        /// <summary>
        /// 下单 API（商户调用）, 生成二维码Url返回给商家
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("api/create")]
        public IActionResult Create([FromBody] MockPayCreateRequest req)
        {
            // 生成全局唯一的128位token和交易Id
            var token = Guid.NewGuid().ToString("N");
            var tx = Guid.NewGuid().ToString("N");

            // code_url 指向扫码打开的页面（模拟微信的 code_url）
            // 这个操作就是根据Http请求的信息生成一个URL，指向当前服务器的/mockpay/{token}路径， 其实正式环境中可以直接用我们项目的域名来访问更好
            //var codeUrl = $"{Request.Scheme}://{Request.Host}/mockpay/{token}";
            // 内网测试，所以用内网IP
            var codeUrl = $"https://192.168.42.157:7109/mockpay/{token}";

            _store[token] = (req.OrderNumber, req.Amount, req.NotifyUrl, tx);

            return Ok(new MockPaymentResponse
            {
                CodeUrl = codeUrl,
                OrderNumber = tx,
                ExpireAt = DateTime.UtcNow.AddMinutes(10)
            });
        }

        // 2) 扫码打开的页面（用户在这里点击支付结果）
        [HttpGet("{token}")]
        public IActionResult Index(string token)
        {
            if (!_store.TryGetValue(token, out var info))
                return NotFound("无效二维码");

            ViewData["Token"] = token;
            ViewData["OrderId"] = info.OrderId;
            ViewData["Amount"] = info.Amount;
            ViewData["AmountString"] = info.Amount.ToString("F2");

            // 转到模拟支付视图页面， 这里是为了适配Controller，以及与本项目的内容做一个区分，便选择了View页面而不是Razor页面
            return View("MockPay"); 
        }

        // 3) 用户在页面点击，平台向商户回调
        [HttpPost("{token}/pay")]
        public async Task<IActionResult> Pay(string token, [FromForm] string result)
        {
            // 查找订单信息是否存在
            if (!_store.TryGetValue(token, out var info))
                return NotFound();

            bool success = result == "success";

            // 调用商户回调 URL（POST JSON）
            var notify = new PaymentNotifyModel
            {
                OrderNumber = info.OrderId,
                TransactionId = info.TransactionId,
                Success = success
            };

            // 忽略异常（生产需重试/签名）
            try
            {
                var resp = await _http.PostAsJsonAsync(info.NotifyUrl, notify);
            }
            catch { /* log */ }

            // 成功返回支付成功Veiw,失败返回支付失败View
            if (success == true)
                return View("PayView");
            else
                return View("PayFail");
        }
    }

}
