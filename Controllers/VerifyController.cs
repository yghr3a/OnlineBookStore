using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Services;

[ApiController]
[Route("api/account/verify")]
public class VerifyController : ControllerBase
{
    private AccountAppliaction _accountAppliaction;

    public VerifyController(AccountAppliaction accountAppliaction)
    {
       _accountAppliaction = accountAppliaction;
    }

    [HttpGet("email")]
    public async Task<IActionResult> VerifyEmail(string token)
    {
        var result = await _accountAppliaction.VerifyUserRegisterTokenAsync(token);

        if(result.IsSuccess == true)
        {
            return Ok("邮箱验证成功");
        }
        else
        {
            return BadRequest(result.ErrorMsg);
        }
    }
}
