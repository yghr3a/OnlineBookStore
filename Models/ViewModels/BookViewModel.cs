using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OnlineBookStore.Models.ViewModels
{
    public class BookViewModel
    {
        // 图书编号
        public int Number { get; set; }
        // 图书名称
        public required string Name { get; set; }
        // 图书作者
        public string? Author { get; set; }
        // 图书价格
        public decimal Price { get; set; }
        // 图书分类
        // 后续应该改为数组类型(数据库自动建表)
        public string? Category { get; set; }
        // 图书介绍
        public string? Introduction { get; set; }
        // 图书封面图片链接
        public string? CoverImageUrl { get; set; }
        // 图书销量
        public int Sales { get; set; }
    }
}
