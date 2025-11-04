namespace OnlineBookStore.Infrastructure
{
    /// <summary>
    /// URL相关的信息设置
    /// </summary>
    public class UrlOptions
    {
        public string Mode { get; set; } = string.Empty;          // URL模式, 可选值: Local, Inner, Public
        public string LocalUrl { get; set; } = string.Empty;         // 本地URL
        public string InnerUrl { get; set; } = string.Empty;         // 内网URL
        public string PublicUrl { get; set; } = string.Empty;        // 公网URL

        public string InnerPort {  get; set; } = string.Empty;     // 内网端口
    }
}
