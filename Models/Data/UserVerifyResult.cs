using System.Security.Claims;

namespace OnlineBookStore.Models.Data
{
    // 用于封装用户验证结果
    // 后续会添加更多属性, 包含更多信息, 比如错误码, 错误信息等
    // [2025/10/10] 添加错误信息属性, 不是用自定义业务异常的原因是想先完成核心的内容先
    public class UserVerifyResult
    {
        public bool IsValid { get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
        public List<Claim> ClaimList { get; set; } = new List<Claim>();
    }
}
