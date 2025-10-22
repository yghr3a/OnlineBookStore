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
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Unfinished;
        // 总计金额
        public float Total => OrderItemViewModels.Sum(x => x.Total);
        // 支付方式
        public PaymentMethod PaymentMethod { get; set; }
        // 订单创建时间
        public DateTime CreatedDate { get; set; }


        // 订单项视图模型列表
        public List<OrderItemViewModel> OrderItemViewModels { get; set; } = new();

        // 总计金额字符串格式化, 保留两位小数
        public string TotalString => Total.ToString("F2");
        // 订单状态字符串
        public string OrderStatusString => OrderStatus == OrderStatus.Finished ? "已完成" : "未完成";
        // 支付方式字符串
        public string PaymentMethodString => PaymentMethod == PaymentMethod.WeChat ? "微信支付" : "支付宝支付";
        // 订单编号字符串
        public string OrderNumberString => "No." + Number.ToString("D8");

    }
}
