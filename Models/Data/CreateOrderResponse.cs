namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 创建订单请求类型, 作为创建订单的参数类型
    /// </summary>
    public class CreateOrderResponse
    {
        public List<CreateOrderItem> Items { get; set; } = new();
    }
}
