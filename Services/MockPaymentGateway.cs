using QRCoder;
using System.Collections.Concurrent;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 模拟支付网关, 本类会模拟支付时的第三方支付网关行为 
    /// </summary>
    /// 后续考虑增加重试机制, 增加策略工厂等
    public class MockPaymentGateway

    {
        private readonly HttpClient _http;
        private UrlFactory _urlFactory;


        // 这里直接使用IConfiguration读取配置, 先以实现为目标
        // 后续可以考虑使用专门的配置类来绑定配置节, 不容易犯"魔法字符串"的错误
        private readonly IConfiguration _cfg; // 可从配置读取mock平台地址

        public MockPaymentGateway(HttpClient http, UrlFactory urlFactory, IConfiguration cfg)
        {
            _http = http;
            _cfg = cfg;
            _urlFactory = urlFactory;
        }

        /// <summary>
        /// 创建支付方法
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<DataResult<MockPaymentResponse>> CreatePaymentAsync(Order order)
        {
            var orderAmount = order.OrderItems.Sum(oi => oi.Price * oi.Count);
            var createReq = new MockPayCreateRequest
            {
                OrderNumber = order.Number.ToString(),
                Amount = (decimal)orderAmount,
                MerchantId = _cfg["MockPayment:MerchantId"] ?? "demo_shop",
                // TODO:notifyUrl的警告处理
                NotifyUrl = _urlFactory.GetNofiyUrl()
            };

            // 获取模拟第三方支付平台的api
            var apiUrl = _urlFactory.GetMockPaymentCreateUrl();
            var res = await _http.PostAsJsonAsync(apiUrl, createReq);
            // TODO:这里的异常处理可以更完善一些, 目前Application负责兜底捕获并处理
            res.EnsureSuccessStatusCode();

            var model = await res.Content.ReadFromJsonAsync<MockPaymentResponse>();
            if (model is null)
                return DataResult<MockPaymentResponse>.Fail("调用第三方支付平台API结果为空");

            var orderQr = new MockPaymentResponse
            {
                OrderNumber = order.Number.ToString(),  // 这里坑定不是用model里的OrderNumber了， 那里面的是交易Id， 后面在给他补充上去
                CodeUrl = model.CodeUrl,
                ExpireAt = model.ExpireAt
            };

            return DataResult<MockPaymentResponse>.Success(orderQr);
        }
    }
}
