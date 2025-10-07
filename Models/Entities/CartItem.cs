using System.Data;

namespace OnlineBookStore.Models.Entities
{
    public class CartItem : IEntityModel
    {
        // 购物车项Id
        public int Id { get; set; }
        // 关联的购物车Id
        public required int CartId { get; set; }
        // 购物车项编号
        public required int Number { get; set; }
        // 添加到购物车的时间
        public required DateTime CreatedDate { get; set; }
        // 关联的图书Id
        public required int BookId { get; set; }
        // 购买的数量
        public required string Count { get; set; }
    }
}
