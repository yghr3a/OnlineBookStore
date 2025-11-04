using Microsoft.AspNetCore.Identity;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 用户工厂
    /// </summary>
    public class UserFactory
    {
        private NumberFactory _numberFactory;
        private IPasswordHasher<User> _passwordHasher;

        public UserFactory(NumberFactory numberFactory, IPasswordHasher<User> passwordHasher)
        {
            _numberFactory = numberFactory;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// 构建用户实体对象
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public DataResult<User> CreateUserEntity(UserRegisterInfo info)
        {
            // 创建新用户的实体模型类对象
            var newUser = new User
            {
                // Number字段后续需要改为更合理的生成方式, 现在只是简单的用时间戳
                Number = _numberFactory.CreateNumber<User>(),
                UserName = info.UserName,
                Email = info.Email,
                UserRole = UserRole.Customer,
                RegistrationDate = DateTime.UtcNow
            };

            // 对密码进行哈希加密
            var passwordHash = _passwordHasher.HashPassword(newUser, info.Password);
            // 将密码赋值进去
            newUser.PasswordHash = passwordHash;

            return DataResult<User>.Success(newUser);
        }
    }
}
