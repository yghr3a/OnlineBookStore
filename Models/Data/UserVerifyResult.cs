using System.Security.Claims;

namespace OnlineBookStore.Models.Data
{
    // 用于封装用户验证结果
    // 后续会添加更多属性, 包含更多信息, 比如错误码, 错误信息等
    public class UserVerifyResult
    {
        public bool IsValid { get; set; }
        public List<Claim> ClaimList { get; set; } = new List<Claim>();
    }
}
