using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 创建订单视图模参数类型
    /// </summary>
    public class CreateOrderViewModelArge
    {
        public required User User {  get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Book> Books { get; set; } = new List<Book>();

    }
}
