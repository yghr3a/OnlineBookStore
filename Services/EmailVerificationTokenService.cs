using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using OnlineBookStore.Infrastructure;
using OnlineBookStore.Models.Data;

/// <summary>
/// 邮箱验证Token服务类
/// </summary>
public class EmailVerificationTokenService
{
    private readonly JWTOptions _jwtOptions;
    private readonly TimeSpan _tokenLifetime;

    public EmailVerificationTokenService(IOptions<JWTOptions> options)
    {
        _jwtOptions = options.Value;
        _tokenLifetime = TimeSpan.FromMinutes(_jwtOptions.TokenLifeTimeMunite);
    }

    /// <summary>
    /// 生成Token方法
    /// [2025/10/29] 因为不涉及数据库操作使用不写成异步方法, 该用户
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public DataResult<string> GenerateToken(string email)
    {
        var claims = new[]
        {
            new Claim("email", email),      // 用户邮箱, Type为"email"
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,        // 签发人, 本应用
            audience: _jwtOptions.Audience,    // 接受人, 可以不写, 因为这是发送给用户的验证Token
            claims: claims,
            expires: DateTime.UtcNow.Add(_tokenLifetime),   // Token时效
            signingCredentials: creds                       // 签名    
        );

        var tokenStr =  new JwtSecurityTokenHandler().WriteToken(token);
        return DataResult<string>.Success(tokenStr);
    }

    /// <summary>
    /// 验证Token并返回邮箱信息方法
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public DataResult<string> ValidateTokenAndReturnEmail(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

        // 验证token, 将Token解析成Claim管道类型
        var claims = handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = key,
            ValidateLifetime = true,
        }, out _);

        // 尝试提取邮箱信息
        var email = claims.FindFirst("email")?.Value;

        if (string.IsNullOrEmpty(email) == true)
            return DataResult<string>.Fail("Token提取邮箱信息结果为空");
        else
            return DataResult<string>.Success(email);
    }
}

