using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Services;
using System.Security.Claims;
using OnlineBookStore.Models.Data;

namespace OnlineBookStore.Pages.Account
{
    public class RegisterModel : PageModel
    {
        // 注册页面的用户名, 邮箱, 密码, 确认密码
        [BindProperty] public string UserName { get; set; } = "";
        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        [BindProperty] public string ConfirmPassword { get; set; } = "";

        // 显示在注册页面的内容盒下面的错误信息
        [BindProperty] public string? BoxDownErrorMessage { get; set; }

        // 负责用户注册相关业务操作的服务
        private AccountAppliaction _accountAppliaction;

        public RegisterModel(AccountAppliaction accountAppliaction)
        {
            _accountAppliaction = accountAppliaction;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 用户名为空的判断
            if (string.IsNullOrWhiteSpace(UserName))
            {
                BoxDownErrorMessage = "用户名为空!";
                ModelState.AddModelError("", "用户名为空");
                return Page();
            }

            // 邮箱为空的判断
            if (string.IsNullOrWhiteSpace(Email))
            {
                BoxDownErrorMessage = "邮箱为空!";
                ModelState.AddModelError("", "邮箱为空");
                return Page();
            }

            // 密码为空的判断
            if (string.IsNullOrWhiteSpace(Password))
            {
                BoxDownErrorMessage = "密码为空";
                ModelState.AddModelError("", "密码为空");
                return Page();
            }

            // 确认密码为空的判断
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                BoxDownErrorMessage = "确认密码为空";
                ModelState.AddModelError("", "确认密码为空");
                return Page();
            }

            // 密码和确认密码不匹配的判断
            if (Password != ConfirmPassword)
            {
                BoxDownErrorMessage = "密码和确认密码不匹配";
                ModelState.AddModelError("", "密码和确认密码不匹配");
                return Page();
            }

            var info = new UserRegisterInfo() { UserName = UserName, Password = Password, Email = Email };
            var userRegisterResult = await _accountAppliaction.UserRegistraionAsync(info);

            if (userRegisterResult.IsSuccess == true)
            {
                // 注册成功, 自动登录
                // [2025/11/4] 现在改为注册成功后转到提示页面, 提示页面自动跳转到登录页面

                // 登录成功, 跳转到首页
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "注册信息提交成功, 请查收您的邮箱, 点击我们发送给您的验证链接, 即可完成注册验证",
                    RedirectUrl = "/Account/Login",
                    Seconds = 10
                });
            }
            else
            {
                BoxDownErrorMessage = userRegisterResult.ErrorMsg;
                ModelState.AddModelError("", $"{userRegisterResult.ErrorMsg}");
                return Page();
            }

        }
    }
}
