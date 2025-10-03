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

        // 图书作者
        public string? Author { get; set; }


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
