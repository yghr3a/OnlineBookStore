using System.Security.Claims;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 用户上下文服务, 用于获取当前请求的用户信息
    /// </summary>
    public class UserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        // 用户编号（数字）
        public string? UserId =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // 用户名
        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        // 邮箱
        public string? Email =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        // ⚠ 使用方案 1：角色 Claim 改为 "role"
        public string? RawRoleString =>
            _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value;
    }
}
