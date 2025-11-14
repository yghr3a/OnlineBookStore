namespace OnlineBookStore.Models.Data
{
    // 与支付操作的相关数据模型

    /// <summary>
    /// 模拟第三方支付响应模型
    /// </summary>
    public class MockPaymentResponse
    {
        // 虽然本次项目里的Id和Number之类属性的类型为int, 但实际生产环境中建议使用string类型以支持更大范围的值
        // 比如分布式Id生成器生成的Id通常是string类型
        // 所以这里的OrderNumber也使用string类型, 而目前项目里的In和Number类型均为int的问题就先不修改了, 签下技术债了
        public string OrderNumber { get; set; } = default!;
        public string CodeUrl { get; set; } = default!; // 给前端生成二维码的链接
        public DateTime ExpireAt { get; set; }
    }

    /// <summary>
    /// 模拟支付创建请求模型
    /// </summary>
    public class MockPayCreateRequest
    {
        // string类型以支持更大范围的值, 也是实际开发中常见的做法
        public string OrderNumber { get; set; } = default!;     // 订单编号
        public decimal Amount { get; set; }                     // 金额
        public string MerchantId { get; set; } = default!;      // 商家Id, 模拟第三方支付平台分配的商家Id
        public string NotifyUrl { get; set; } = default!;       // 当商家支付成功或失败后, 本项目提供给第三方支付平台回调的地址
    }

    /// <summary>
    /// 模拟支付创建响应模型
    /// </summary>
    /// [2025/11/4] 这个类模拟第三方支付平台返回的结果, 但是这样的话, 作为PlaceOrderAsync方法的返回值的一部分返回给前端的
    /// 所以决定给这个类改变名字, 以免和实际返回给前端的类混淆
    /// 现在是PlaceOrderPaymentResponse, 下单支付响应模型
    public class PlaceOrderPaymentResponse
    {
        public string CodeUrl { get; set; } = default!;         // 二维码Url
        public string TransactionId { get; set; } = default!;   // 交易Id, 模拟第三方支付平台返回的交易Id
        public DateTime ExpireAt { get; set; }                  // 二维码过期时间
    }

    public class PaymentNotifyModel
    {
        // string类型以支持更大范围的值, 也是实际开发中常见的做法
        public string OrderNumber { get; set; } = default!;     // 订单编号
        public string TransactionId { get; set; } = default!;   // 交易Id, 模拟第三方支付平台返回的交易Id
        public bool Success { get; set; }
    }
}
