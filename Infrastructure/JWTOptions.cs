namespace OnlineBookStore.Infrastructure
{
    /// <summary>
    /// JWT相关的信息设置
    /// </summary>
    public class JWTOptions
    {
        public string Secret { get; set; } = string.Empty;      // JWT原始密钥
        public string Issuer {  get; set; } = string.Empty;     // 签发人
        public string Audience {  get; set; } = string.Empty;   // 接收人
        public int TokenLifeTimeMunite {  get; set; }           // Token有效时间(分钟)
    }
}
