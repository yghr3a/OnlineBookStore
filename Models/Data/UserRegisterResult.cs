namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 用户注册结果
    /// </summary>
    public class UserRegisterResult
    {
        // 是否注册成功
        public bool IsSuccess { get; set; }

        // 注册用于验证邮箱或者手机号码返回的Token
        // 暂时不用
        public string Token { get; set; } = string.Empty;

        // 错误信息
        public string ErrorMsg { get; set; } = string.Empty;
    }
}
