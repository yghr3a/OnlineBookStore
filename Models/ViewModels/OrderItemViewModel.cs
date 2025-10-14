namespace OnlineBookStore.Models.ViewModels
{
    /// <summary>
    /// 订单项视图模型
    /// </summary>
    public class OrderItemViewModel
    {
        // 订单项编号
        public int Number {  get; set; }
        // 所隶属的订单的编号
        public int OrderNumber { get; set; }
        // 所对应的书籍编号
        public int BookNumber { get; set; }


        // 书籍标题
        public required string BookTitle { get; set; }
        // 书籍封面图片链接
        public string BookCoverImageUrl { get; set; } = string.Empty;
        // 书籍单价
        public required float Price { get; set; }
        // 数量
        public required int Count { get; set; }
        // 总价
        public float Total => Price * Count;


        // 单价字符串格式化, 保留两位小数
        public string PriceString { get { return Price.ToString("F2"); } }
        // 总价字符串格式化, 保留两位小数
        public string TotalString { get { return Total.ToString("F2"); } }
    }
}
