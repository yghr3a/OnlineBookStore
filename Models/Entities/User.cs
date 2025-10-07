using OnlineBookStore.Models;

namespace OnlineBookStore.Models.Entities
{
    public class User
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
        public required string PasswordHash { get; set; }
        // 电子邮箱(如果需要电话号码的话, 可以做多种注册方法, 而这种用于验证用户账户唯一性的信息也需要确保拥有至少有一条)
        public string? Email { get; set; }
        // 用户注册时间
        public required DateTime RegistrationDate { get; set; }
        // 用户购物车
        // public Cart CartItems { get; set; }
        // 用户订单(历史订单)
        // public List<Order> Orders { get; set; }
    }
}
