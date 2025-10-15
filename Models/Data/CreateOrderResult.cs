namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 创建订单结果信息
    /// </summary>
    public class CreateOrderResult
    {
        public bool IsSuccessed { get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
    }
}
