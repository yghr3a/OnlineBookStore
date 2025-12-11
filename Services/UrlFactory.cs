using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using OnlineBookStore.Infrastructure;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// URL工厂类, 根据配置的URL模式生成对应的URL
    /// </summary>
    public class UrlFactory
    {
        private UrlOptions _options;
        private string _url;
        private string _mockPaymentBaseUrl;
        private readonly IConfiguration _cfg; // 可从配置读取mock平台地址

        public UrlFactory(IOptions<UrlOptions> options, IConfiguration cfg)
        {
            _options = options.Value;
            _cfg = cfg;

            // 根据配置的模式选择对应的URL, 这样只需要在appsettings.json中修改Mode即可切换URL模式
            if (_options.Mode == "Public")
                _url = _options.PublicUrl;
            else if (_options.Mode == "Inner")
                // 如果是内网,那就根据本机内网IP和配置的端口来生成URL
                _url = "https://" + LocalNetworkHelper.GetLocalIPv4() + ":" + _options.InnerPort + "/";
            else
                _url = _options.LocalUrl;

            // 根据配置生成模拟支付平台的基础URL
            if (_options.Mode == "Public")
                // 公网用公网地址
                _mockPaymentBaseUrl = _options.PublicUrl;
            else // 除此之外都是用内网地址
                _mockPaymentBaseUrl = _url;

        }

        /// <summary>
        /// 创建带有令牌的确认URL
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string CreateTokenUrl(string route, string token)
        {
            return $"{_url}{route}?token={token}";
        }

        /// <summary>
        /// 创建带有多个参数的URL
        /// </summary>
        /// <param name="route"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string CreateParasUrl(string route, Dictionary<string, string> paras)
        {
            var query = string.Join("&", paras.Select(kv => $"{kv.Key}={kv.Value}"));
            return $"{_url}{route}?{query}";
        }

        /// <summary>
        /// 获取支付通知回调URL, 根据Model配置的URL模式生成对应的URL
        /// </summary>
        /// <returns></returns>
        public string GetNofiyUrl()
        {
            if(_options.Mode == "Public")
                return $"{_options.PublicUrl}{_cfg["MockPayment:NofiyUrlWithoutDomain"]}";
            else
                return $"{_options.LocalUrl}{_cfg["MockPayment:NofiyUrlWithoutDomain"]}";
        }

        /// <summary>
        /// 获取模拟支付创建支付URL, 根据Model配置的URL模式生成对应的URL
        /// </summary>
        /// <returns></returns>
        public string GetMockPaymentCreateUrl()
        {
            // 只有公网配置时使用PublicUrl, 内网测试时因为没有SSL证书, 所以不能用内网IP, 只能用本地Localhost地址
            if(_options.Mode == "Public")
                return $"{_options.PublicUrl}{_cfg["MockPayment:MockPaymentCreateUrlWithoutDomain"]}";
            else
                return $"{_options.LocalUrl}{_cfg["MockPayment:MockPaymentCreateUrlWithoutDomain"]}";
        }


        /// <summary>
        /// 创建带有令牌的第三方支付平台url的支付url
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string CreateTokenMockPayUrl(string route, string token)
        {
            return $"{_mockPaymentBaseUrl}{route}/{token}";
        }
    }

}
