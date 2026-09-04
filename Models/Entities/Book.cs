using OnlineBookStore.Models.Exceptions;

namespace OnlineBookStore.Models.Entities
{
    public enum BookStatus
    {
        OnDraft,      // 草稿状态, 还未上架
        OnSale,       // 销售状态, 已经上架
        OnOffSale,    // 下架状态, 已经下架
    }


    public class Book : IEntityModel
    {
        // 图书Id
        public int Id { get; set; }

        // 图书编号
        public required int Number { get; set; }

        // 图书状态
        public required BookStatus Status { get; set; } = BookStatus.OnDraft;

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


        // [2026/5/17] 开始为Books实体添加一些方法, 尝试过度到充血模型, 目前只简单添加下面的

        /// <summary>
        /// 将图书上架, 下架状态和草稿都能进行
        /// </summary>
        public void OnSale()
        {
            if (Status == BookStatus.OnSale)
                throw new DomainException("图书已经处于销售状态");

            Status = BookStatus.OnSale;
        }

        /// <summary>
        /// 将图书下架, 只能在销售状态下进行
        /// </summary>
        public void OffSale()
        {
            if (Status != BookStatus.OnSale)
                throw new DomainException("只有销售状态的图书才能下架");

            Status = BookStatus.OnOffSale;
        }

        /// <summary>
        /// 销量增加方法
        /// </summary>
        /// <param name="count"></param>
        public void IncreaseSales(int count)
        {
            if (count < 0)
                throw new DomainException("试图增加负数的销量");

            Sales += count;
        }

    }
}
