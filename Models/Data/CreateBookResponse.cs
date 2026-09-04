using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 图书创建请求
    /// </summary>
    public class CreateBookResponse
    {
        // 图书名称
        public required string Name { get; set; }

        // 图书作者
        public required string Authors { get; set; } = string.Empty;

        // 出版社
        public required string Publisher { get; set; } = string.Empty;

        // 出版年份
        public required int PublishYear { get; set; }

        // 图书类别
        public required string Categorys { get; set; } = string.Empty;

        // 图书介绍
        public required string Introduction { get; set; } = string.Empty;

        // 图书封面图片链接
        public required string CoverImageUrl { get; set; } = string.Empty;

        // 图书价格
        public required decimal Price { get; set; }

        // 图书销量
        public required int Sales { get; set; } = 0;
    }
}
