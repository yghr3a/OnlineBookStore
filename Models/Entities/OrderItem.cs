using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStore.Models.Entities
{
    /// <summary>
    /// 订单项
    /// </summary>
    public class OrderItem : IEntityModel
    {
        // 订单项Id
        public int Id { get; set; }

        // 所隶属的Order的Id
        [ForeignKey("Order")]
        public int OrderId {  get; set; }
        // 导航属性
        public Order? Order { get; set; }

        // 书籍Id
        [ForeignKey("Book")]
        public int BookId { get; set; }
        // 导航属性
        public Book? Book { get; set; }

        // 购买时的单价
        public decimal? Price { get; set; }
        // 购买数量
        public int Count {  get; set; }
    }
}
