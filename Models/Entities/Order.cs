using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStore.Models.Entities
{
    // 订单状态
    public enum OrderStatus
    {
        None = 0,
        WaitingForPayment, //等待支付
        Fail,              //支付失败
        Success,           //支付成功
    }

    // 支付方式
    public enum PaymentMethod
    {
        WeChat,     //   微信支付
        AplyPay     //   支付宝支付
    }

    public class Order : IEntityModel
    {
        // 订单Id
        public int Id { get; set; }
        // 订单编号
        public int Number { get; set; }

        // 所隶属的用户Id, 外键
        [ForeignKey("User")]
        public int UserId {  get; set; } 
        public User? User { get; set; }

        // 订单状态
        public OrderStatus OrderStatus { get; set; } = OrderStatus.WaitingForPayment;
        // 支付方式
        public PaymentMethod PaymentMethod { get; set; }
        // 订单创建时间
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;


        // 导航属性, 订单项列表
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
