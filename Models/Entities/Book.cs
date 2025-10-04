namespace OnlineBookStore.Models.Entities
{
    public class Book : IEntityModel
    {
        // 图书Id
        public int Id { get; set; }

        // 图书编号
        public required int Number { get; set; }

        // 图书名称
        public required string Name { get; set; }

        // 图书作者(可能不止一位)
        public List<string?>? Authors { get; set; }

        // 出版社
        public string? Publisher { get; set; }

        // 出版年份
        public int? PublishYear { get; set; }

        // 图书类别
        public List<string?>? Categorys { get; set; }

        // 图书介绍
        public string? Introduction { get; set; }

        // 图书封面图片链接
        public string? CoverImageUrl { get; set; }

        // 图书价格
        public decimal Price { get; set; }

        // 图书销量
        public required int Sales { get; set; } = 0;
    }
}
