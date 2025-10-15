namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 创建订单请求类型单项
    /// </summary>
    public class CreateOrderItem
    {
        // 书籍编号
        public int BookNumber { get; set; }
        // 购买数量
        public int Count { get; set; } = 1;
    }
}
