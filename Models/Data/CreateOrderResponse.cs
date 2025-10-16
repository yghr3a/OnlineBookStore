using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 创建订单请求类型, 作为创建订单的参数类型
    /// </summary>
    public class CreateOrderResponse
    {
        // 支付方式
        // [2025/10/16]为了便利, 添加一些行为逻辑, 虽然算是领域类的一部分
        public PaymentMethod PaymentMethod { get; set; }
        // 订单状态
        public OrderState OrderState { get; set; } = OrderState.Unfinished;
        // 订单单项
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
