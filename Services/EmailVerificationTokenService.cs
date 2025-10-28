using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class EmailVerificationTokenService
{
    private readonly string _jwtSecret;
    private readonly TimeSpan _tokenLifetime = TimeSpan.FromHours(2); // 有效期 2 小时

    public EmailVerificationTokenService(IConfiguration config)
    {
        _jwtSecret = config["Jwt:Secret"]!;
    }

    /// <summary>
    /// 生成Token方法
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public string GenerateToken(string email)
    {
        var claims = new[]
        {
            new Claim("email", email),      // 用户邮箱, Type为"email"
            new Claim("purpose", "email_verification"),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "OnlineBookStore",
            audience: "OnlineBookStore",
            claims: claims,
            expires: DateTime.UtcNow.Add(_tokenLifetime),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 验证Token方法
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public string? ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));

        try
        {
            var claims = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = key,
                ValidateLifetime = true,
            }, out _);

            return claims.FindFirst("email")?.Value;
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }
}

