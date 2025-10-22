using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Models.ViewModels
{
    /// <summary>
    /// 订单视图模型
    /// </summary>
    public class OrderViewModel
    {
        // 订单编号
        public int Number { get; set; }
        // 所隶属用户的编号
        public int UserNumber { get; set; }


        // 订单状态
        public OrderState OrderState { get; set; } = OrderState.Unfinished;
        // 总计金额
        public float Total => OrderItemViewModels.Sum(x => x.Total);
        // 支付方式
        public PaymentMethod PaymentMethod { get; set; }
        // 订单创建时间
        public DateTime CreatedDate { get; set; }
        

        // 订单项视图模型列表
        public List<OrderItemViewModel> OrderItemViewModels { get; set; } = new();
    }
}
