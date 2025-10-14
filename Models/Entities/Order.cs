using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStore.Models.Entities
{
    // 订单状态
    public enum OrderState
    {
        Unfinished, //未完成
        Finished    //已完成
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

        // 所隶属的用户Id, 外键
        [ForeignKey("User")]
        public int UserId {  get; set; } 
        public User? User { get; set; }

        // 订单状态
        public OrderState OrderState { get; set; } = OrderState.Unfinished;
        // 支付方式
        public PaymentMethod PaymentMethod { get; set; }
        // 订单创建时间
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;


        // 导航属性, 订单项列表
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
