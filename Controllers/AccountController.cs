using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Services;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly EmailVerificationTokenService _tokenService;
    private readonly UserDomainService _userDomainService;

    public AccountController(EmailVerificationTokenService tokenService, UserDomainService userDomainService)
    {
        _tokenService = tokenService;
        _userDomainService = userDomainService;
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(string token)
    {
        var email = _tokenService.ValidateToken(token);
        if (email == null)
            return BadRequest("验证链接无效或已过期。");

        // 找到用户并更新状态
        var userRes = await _userDomainService.GetUserByEmailAsync(email);
        if (userRes.IsSuccess == false)
            return BadRequest("用户不存在。");

        var user = userRes.Data!;
        //user.IsEmailVerified = true;
        //await _userDomainService.UpdateAsync(user);

        return Ok("邮箱验证成功！");
    }
}
