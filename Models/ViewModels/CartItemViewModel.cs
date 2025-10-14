namespace OnlineBookStore.Models.ViewModels
{
    /// <summary>
    /// 购物单项视图模型
    /// </summary>
    public class CartItemViewModel
    {
        // 编号, 暂时直接用ID替代一下
        public required int Number { get; set; }
        // 书籍编号
        public required int BookNumber { get; set; }

        // [2025/10/13] 要不干脆直接搞一个BookViewModel对象在这里算了

        // 书籍标题
        public required string BookTitle { get; set; }
        // 书籍封面图片链接
        public string BookCoverImageUrl { get; set; } = string.Empty;
        // 书籍作者
        public string BookAuthor { get; set; } = string.Empty;
        // 书籍单价
        public required float Price { get; set; }
        // 数量
        public required int Count { get; set; }
        // 总价
        public float Total => Price * Count;
        // 添加日期
        public required DateTime AddedDate {get; set;}
        // 单价字符串格式化, 保留两位小数
        public string PriceString { get { return Price.ToString("F2"); } }
        // 总价字符串格式化, 保留两位小数
        public string TotalString { get { return Total.ToString("F2"); } }
    }
}
