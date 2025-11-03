using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models;
using OnlineBookStore.Respository;
using System.Security.Claims;
using OnlineBookStore.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace OnlineBookStore.Services
{
    public class UserDomainService
    {
        private Respository<User> _responsity;
        private UserContext _userContext;
        private IPasswordHasher<User> _passwordHasher;

        public UserDomainService(Respository<User> repository, UserContext userContext, IPasswordHasher<User> passwordHasher)
        {
            _responsity = repository;
            _userContext = userContext;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// 更新用户实体模型信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<InfoResult> UpdateAsync(User user)
        {
            // 如果设计到应用层需要更新数据项的属性的话, 领域服务类需要提供方法供应用类实现数据更新, 不能让应用类直接依赖仓储类
            _responsity.Update(user);
            await _responsity.SaveAsync();

            return InfoResult.Success();
        }

        /// 获取当前登录用户实体模型
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult<User>> GetCurrentUserEntityModelAsync()
        {
            if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserName))
                return DataResult<User>.Fail("用户不存在");

            var query = _responsity.AsQueryable().Where(u => u.UserName == _userContext.UserName);
            var user = await _responsity.GetSingleByQueryAsync(query);

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
            var query = _responsity.AsQueryable().Where(u => u.Id == userId);
            var user = await _responsity.GetSingleByQueryAsync(query);
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
            var query = _responsity.AsQueryable().Where(u => u.Number == userNumber);
            var user = await _responsity.GetSingleByQueryAsync(query);
            if (user is null)
                return DataResult<int>.Fail("用户不存在");
            return DataResult<int>.Success(user.Id);
        }

        /// <summary>
        /// 通过邮箱查找用户
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<DataResult<User>> GetUserByEmailAsync(string email)
        {
            var query = _responsity.AsQueryable().Where(u => u.Email == email);
            var user = await _responsity.GetSingleByQueryAsync(query);
            if (user is null)
                return DataResult<User>.Fail("用户不存在");
            return DataResult<User>.Success(user);
        }
    }
}
