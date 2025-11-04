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

        public UrlFactory(IOptions<UrlOptions> options) 
        {
            _options = options.Value;

            // 根据配置的模式选择对应的URL, 这样只需要在appsettings.json中修改Mode即可切换URL模式
            if (_options.Mode == "Public")
                _url = _options.PublicUrl;
            else if (_options.Mode == "Inner")
                _url = "https://"+ LocalNetworkHelper.GetLocalIPv4()+ ":" + _options.InnerPort + "/";
            else
                _url = _options.LocalUrl;
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
    }
}
