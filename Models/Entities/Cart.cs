using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStore.Models.Entities
{
    public class Cart : IEntityModel
    {
        // 关联的用户Id
        // UserId 改为主键, 一对一关系, 可以解决相互依赖的问题
        // user对象创建时只需要直接new一个cart对象即可, EFCore框架会负责将其插入到Carts表中, 并且将UserId设置为对应的用户Id
        [Key, ForeignKey("User")] // Cart的主键同时是外键
        public int UserId { get; set; }

        // 导航属性, 关联的用户
        public User? User { get; set; }
        // 购物车中的购物车项
        // [2025/10/12] 改为非空属性, 避免了空引用的问题
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
