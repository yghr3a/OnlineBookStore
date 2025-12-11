using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 书籍领域模型
    /// </summary>
    public class BookDomainService : DomainService<Book>
    {
        public BookDomainService(Repository<Book> bookRespository)
            : base(bookRespository)
        {

        }

        /// <summary>
        /// 根据书籍编号列表获取图书实体模型列表
        /// [2025/10/16] 职责调整, 转交给领域服务负责
        /// </summary>
        /// <param name="BookNumbers"></param>
        /// <returns></returns>
        public async Task<DataResult<List<Book>>> GetBookByNumberAsync(List<int> BookNumbers)
        {
            var booksQurey = _repository.AsQueryable().Where(b => BookNumbers.Contains(b.Number));
            var books = await _repository.GetListByQueryAsync(booksQurey);
            var ErrorMsg = string.Empty;

            foreach (var n in BookNumbers)
                if (books.Find(b => b.Number == n) is null)
                    ErrorMsg += $"编号{n}:书籍不存在\n";

            if (ErrorMsg != string.Empty)
                return DataResult<List<Book>>.Fail(ErrorMsg.Trim());

            return DataResult<List<Book>>.Success(books);
        }

        /// <summary>
        /// 根据图书编号获取单个图书实体模型
        /// [2025/10/16] 职责调整, 转交给领域服务负责
        /// </summary>
        /// <param name="BookNumbers"></param>
        /// <returns></returns>
        public async Task<DataResult<Book>> GetBookByNumberAsync(int BookNumber)
        {
            var booksQurey = _repository.AsQueryable().Where(b => b.Number == BookNumber);
            var book = await _repository.GetSingleByQueryAsync(booksQurey);

            if (book is null)
                return DataResult<Book>.Fail($"编号{BookNumber}:书籍不存在");

            return DataResult<Book>.Success(book);
        }

        /// <summary>
        /// 根据图书Id列表获取图书实体模型列表
        /// </summary>
        /// <param name="bookIds"></param>
        /// <returns></returns>
        public async Task<DataResult<List<Book>>> GetBookByIdAsync(List<int> bookIds)
        {
            var booksQurey = _repository.AsQueryable().Where(b => bookIds.Contains(b.Id));
            var books = await _repository.GetListByQueryAsync(booksQurey);
            var ErrorMsg = string.Empty;
            foreach (var n in bookIds)
                if (books.Find(b => b.Id == n) is null)
                    ErrorMsg += $"Id为{n}的书籍不存在\n";

            if (ErrorMsg != string.Empty)
                return DataResult<List<Book>>.Fail(ErrorMsg.Trim());

            return DataResult<List<Book>>.Success(books);
        }

        /// <summary>
        /// 根据图书Id获取单个图书实体模型
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public async Task<DataResult<Book>> GetBookByIdAsync(int bookId)
        {
            var book = await _repository.GetByIdAsync(bookId);
            if (book is null)
                return DataResult<Book>.Fail($"Id为{bookId}的书籍不存在");
            return DataResult<Book>.Success(book);
        }

        /// <summary>
        /// 根据关键字搜索图书实体模型列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<DataResult<List<Book>>> GetSearchedBooksByKeywordAync(string keyword, int pageIndex = 1, int pageSize = 50)
        {
            var query = _repository.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(b => 
                    b.Name.Contains(keyword) || 
                    b.Authors.Contains(keyword));
            }

            // 分页获取搜索到的书籍实体模型
            var searchedBookEMs = await _repository.GetPagedAsync(query, pageIndex, pageSize);
            return DataResult<List<Book>>.Success(searchedBookEMs);
        }

        /// <summary>
        /// 获取热销数据实体模型
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<DataResult<List<Book>>> GeGetPopularBooksAsync(int pageIndex = 1, int pageSize = 50)
        {
            // 获取书籍仓储的可查询对象
            var query = _repository.AsQueryable();
            // 查询并按销量降序排序
            var popularQuery = query.OrderByDescending(b => b.Sales);
            // 分页获取热销书籍实体模型
            var popularBooks = await _repository.GetPagedAsync(popularQuery, pageIndex, pageSize);
            return DataResult<List<Book>>.Success(popularBooks);
        }
    }
}
