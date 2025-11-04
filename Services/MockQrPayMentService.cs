using QRCoder;
using System.Collections.Concurrent;
using OnlineBookStore.Models.Data;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 模拟二维码支付服务
    /// </summary>
    public class MockQrPayMentService
    {
        // 模拟支付状态存储（token -> 是否支付成功）
        private static readonly ConcurrentDictionary<string, bool?> _payments = new();
        private UrlFactory _urlFactory;

        public MockQrPayMentService(UrlFactory urlFactory)
        {
            _urlFactory = urlFactory;
        }

        /// <summary>
        /// 创建支付请求，返回二维码的Base64字符串
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public DataResult<string> CreatePaymentAsync(int orderId, decimal amount)
        {
            string token = Guid.NewGuid().ToString("N");
            _payments[token] = null;

            string fakePayUrl = _urlFactory.CreateParasUrl("api/pay", new Dictionary<string, string>
            {
                { "token", token },
                { "orderId", orderId.ToString() },
            });
            string qrBase64 = GenerateQrCodeBase64(fakePayUrl);
            return DataResult<string>.Success(qrBase64);
        }

        /// <summary>
        /// 检查支付状态
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool CheckPaymentStatusAsync(string token)
        {
            if (_payments.TryGetValue(token, out var status) && status == true)
                return true;

            return false;
        }

        /// <summary>
        /// 设置支付状态（仅用于模拟）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="isSuccess"></param>
        public static void SetPaymentStatus(string token, bool isSuccess)
        {
            _payments[token] = isSuccess;
        }

        /// <summary>
        /// 生成二维码的Base64字符串
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GenerateQrCodeBase64(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
        }
    }
}
