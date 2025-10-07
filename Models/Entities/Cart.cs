namespace OnlineBookStore.Models.Entities
{
    public class Cart : IEntityModel
    {
        // 购物车项Id
        public int id { get; set; }

        // 关联的用户Id
        public required int UserId { get; set; }

        // 购物车中的购物车项
        public required List<CartItem> CartItems { get; set; }

    }
}
