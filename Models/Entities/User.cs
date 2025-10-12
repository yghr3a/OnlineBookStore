using OnlineBookStore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStore.Models.Entities
{
    public class User : IEntityModel
    {
        // 用户Id
        public int Id { get; set; }
        // 用户编号
        public required int Number { get; set; }
        // 用户名
        public required string UserName { get; set; }

        // 用户角色
        public required Role UserRole { get; set; }
        // 密码哈希值
        // [2025/10/11] 改为可能为空, required太不灵活了
        public string? PasswordHash { get; set; } = string.Empty; 
        // 电子邮箱(如果需要电话号码的话, 可以做多种注册方法, 而这种用于验证用户账户唯一性的信息也需要确保拥有至少有一条)
        public string? Email { get; set; }
        // 用户注册时间
        public required DateTime RegistrationDate { get; set; }

        // 用户购物车
        // [2025/10/11] EFCore框架会负责将其插入到Carts表中, 并且将UserId设置为对应的用户Id
        // [2025/10/12] 改为非空属性, 这样在创建User对象时就必须提供一个Cart对象, 避免了空引用的问题
        public Cart Cart { get; set; } = new Cart();

        // 用户订单(历史订单)
        // public List<Order> Orders { get; set; }
    }
}
