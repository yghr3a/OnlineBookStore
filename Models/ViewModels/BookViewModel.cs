using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OnlineBookStore.Models.ViewModels
{
    public class BookViewModel
    {
        // 图书Id, 为了便于搜索指定图书
        public required int Id { get; set; }

        // 图书编号
        public required int Number { get; set; }

        // 图书名称
        public required string Name { get; set; }

        // 图书作者(可能不止一位)
        public string Authors { get; set; } = string.Empty;

        // 出版社
        public string Publisher { get; set; } = string.Empty;

        // 出版年份
        public int? PublishYear { get; set;}

        // 图书分类
        // 后续应该改为数组类型(数据库自动建表)
        public string Category { get; set; } = string.Empty;

        // 图书介绍
        public string Introduction { get; set; } = "暂无介绍";

        // 图书封面图片链接
        public string CoverImageUrl { get; set; } = string.Empty;

        // 图书价格
        public required decimal Price { get; set; }

        // 格式化后的图书价格字符串, 保留两位小数
        public string PriceString
        {
            get => Price.ToString("F2");
        }

        // 图书销量
        public required int Sales { get; set; } = 0;
    }
}
