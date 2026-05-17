using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 图书领域服务, 负责处理图书相关的业务逻辑, 主要面向是数据写入的逻辑
    /// </summary>
    public class BookDomainService : DomainService<Book>
    {
        public BookDomainService(Repository<Book> bookRespository) 
            : base(bookRespository)
        {

        }

        public async Task<InfoResult> AddBookAsync(Book book)
        {
            // 这里可以添加书籍添加操作的业务逻辑, 但目前没有

            await _repository.AddAsync(book);
            await _repository.SaveAsync();

            return InfoResult.Success();
        }
    }
}
