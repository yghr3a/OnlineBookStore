namespace OnlineBookStore.Models.Entities
{
    public enum BookStatus
    {
        OnDraft,      // 草稿状态, 还未上架
        OnSale,       // 销售状态, 已经上架
        OnOffSale,     // 下架状态, 已经下架
    }


    public class Book : IEntityModel
    {
        // 图书Id
        public int Id { get; set; }

        // 图书编号
        public required int Number { get; set; }

        // 图书状态
        public required BookStatus Status { get; set; } = BookStatus.OnSale;

        // 图书名称
        public required string Name { get; set; }

        // 图书作者(可能不止一位)(EFCore自动建立子表)
        public string Authors { get; set; } = string.Empty;

        // 出版社
        public string Publisher { get; set; } = string.Empty;

        // 出版年份
        public int PublishYear { get; set; }

        // 图书类别(EFCore自动建立子表)
        public string Categorys { get; set; } = string.Empty;

        // 图书介绍
        public string Introduction { get; set; } = string.Empty;

        // 图书封面图片链接
        public string CoverImageUrl { get; set; } = string.Empty;

        // 图书价格
        public required decimal Price { get; set; }

        // 图书销量
        public required int Sales { get; set; } = 0;
    }
}
