using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace OnlineBookStore.Models.Entities
{
    public class CartItem : IEntityModel
    {
        // 购物车项Id
        public int Id { get; set; }
        // 购物车项目编号
        // public int Number { get; set; } = DateTime.Now

        // 关联的购物车Id, 属于外键
        [ForeignKey("Cart")]
        public required int CartId { get; set; }
        // 所属购物车的导航属性
        public Cart? Cart { get; set; }

        // 关联的图书Id, 也属于外键
        [ForeignKey("Book")]
        public required int BookId { get; set; }
        // 对应书籍的导航属性
        public Book? Book { get; set; }

        // 购买的数量, 默认为1
        public required int Count { get; set; } = 1;
        // 添加到购物车的时间
        public required DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
