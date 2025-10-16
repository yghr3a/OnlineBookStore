namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 创建订单请求类型单项
    /// [2025/10/16] 为了类型复用, 更改名字
    /// </summary>
    public class OrderItemDto
    {
        // 书籍编号
        public int BookNumber { get; set; }
        // 购买数量
        public int Count { get; set; } = 1;
    }
}
