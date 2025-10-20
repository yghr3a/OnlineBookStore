using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 书籍领域模型
    /// </summary>
    public class BookDomainService
    {
        private Respository<Book> _bookRespository;

        public BookDomainService(Respository<Book> bookRespository) 
        {
            _bookRespository = bookRespository;
        }

        /// <summary>
        /// 根据书籍编号列表获取图书实体模型列表
        /// [2025/10/16] 职责调整, 转交给领域服务负责
        /// </summary>
        /// <param name="BookNumbers"></param>
        /// <returns></returns>
        public async Task<DataResult<List<Book>>> GetBookByNumberAsync(List<int> BookNumbers)
        {
            var booksQurey = _bookRespository.AsQueryable().Where(b => BookNumbers.Contains(b.Number));
            var books = await _bookRespository.GetListByQueryAsync(booksQurey);
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
            var booksQurey = _bookRespository.AsQueryable().Where(b => b.Number == BookNumber);
            var book = await _bookRespository.GetSingleByQueryAsync(booksQurey);

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
            var booksQurey = _bookRespository.AsQueryable().Where(b => bookIds.Contains(b.Id));
            var books = await _bookRespository.GetListByQueryAsync(booksQurey);
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
            var book = await _bookRespository.GetByIdAsync(bookId);
            if (book is null)
                return DataResult<Book>.Fail($"Id为{bookId}的书籍不存在");
            return DataResult<Book>.Success(book);
        }


    }
}
