using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 创建订单请求类型, 作为创建订单的参数类型
    /// </summary>
    public class CreateOrderResponse
    {
        // 支付方式
        public PaymentMethod PaymentMethod { get; set; }
        // 订单状态
        public OrderState OrderState { get; set; } = OrderState.Unfinished;
        // 订单单项
        public List<CreateOrderItem> Items { get; set; } = new();
    }
}
