using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models;
using OnlineBookStore.Repository;
using System.Security.Claims;
using OnlineBookStore.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OnlineBookStore.Services
{
    public class UserDomainService : DomainService<User>
    {
        private UserContext _userContext;
        private IPasswordHasher<User> _passwordHasher;

        public UserDomainService(Repository<User> repository, UserContext userContext, IPasswordHasher<User> passwordHasher)
            : base(repository)
        {
            _userContext = userContext;
            _passwordHasher = passwordHasher;
        }

        /// 获取当前登录用户实体模型
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult<User>> GetCurrentUserEntityModelAsync()
        {
            if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserName))
                return DataResult<User>.Fail("用户不存在");

            var query = _repository.AsQueryable().Where(u => u.UserName == _userContext.UserName);
            var user = await _repository.GetSingleByQueryAsync(query);

            if (user is null) 
                return DataResult<User>.Fail("用户不存在");

            return DataResult<User>.Success(user);
        }

        /// <summary>
        /// 通过用户Id获取用户编号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<DataResult<int>> GetUserNumberByUserIdAsync(int userId)
        {
            var query = _repository.AsQueryable().Where(u => u.Id == userId);
            var user = await _repository.GetSingleByQueryAsync(query);
            if (user is null)
                return DataResult<int>.Fail("用户不存在");
            return DataResult<int>.Success(user.Number);
        }

        /// <summary>
        /// 通过用户编号获取用户Id
        /// </summary>
        /// <param name="userNumber"></param>
        /// <returns></returns>
        public async Task<DataResult<int>> GetUserIdByUserNumberAsync(int userNumber)
        {
            var query = _repository.AsQueryable().Where(u => u.Number == userNumber);
            var user = await _repository.GetSingleByQueryAsync(query);
            if (user is null)
                return DataResult<int>.Fail("用户不存在");
            return DataResult<int>.Success(user.Id);
        }

        /// <summary>
        /// 验证已登录用户的密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public InfoResult VerifyUserPassword(User user, string password)
        {
            // 验证密码, 这里使用ASP.NET Core Identity的密码哈希验证
            var pwHashVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);

            // 如果验证失败
            if (pwHashVerificationResult == PasswordVerificationResult.Failed)
                return InfoResult.Fail("密码错误");

            return InfoResult.Success();
        }

        /// <summary>
        /// 验证用户注册信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<InfoResult> VerifyUserResiterInfo(UserRegisterInfo info)
        {
            // 检查用户名和邮箱的唯一性
            var existingUserQuary = _repository.AsQueryable().Where(u => u.UserName == info.UserName);
            var existingEmailQuary = _repository.AsQueryable().Where(u => u.Email == info.Email);

            var existingUser = await _repository.GetSingleByQueryAsync(existingUserQuary);
            var existingEmail = await _repository.GetSingleByQueryAsync(existingEmailQuary);

            if (existingUser is not null)
                return InfoResult.Fail("该用户名已被注册");

            if (existingEmail is not null)
                return InfoResult.Fail("该邮箱已被注册");

            return InfoResult.Success();
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<InfoResult> AddUser(User user)
        {
            await _repository.AddAsync(user);
            await _repository.SaveAsync();
            return InfoResult.Success();
        }

        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<DataResult<User>> GetUserByUserNameAsync(string userName)
        {
            var query = _repository.AsQueryable().Where(u => u.UserName == userName);
            var user = await _repository.GetSingleByQueryAsync(query);
            if (user is null)
                return DataResult<User>.Fail("用户不存在");
            return DataResult<User>.Success(user);
        }

        /// <summary>
        /// 通过邮箱查找用户
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<DataResult<User>> GetUserByEmailAsync(string email)
        {
            var query = _repository.AsQueryable().Where(u => u.Email == email);
            var user = await _repository.GetSingleByQueryAsync(query);
            if (user is null)
                return DataResult<User>.Fail("用户不存在");
            return DataResult<User>.Success(user);
        }
    }
}
