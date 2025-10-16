using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Respository;

namespace OnlineBookStore.Services
{
    // 图书服务, 提供与图书相关的业务逻辑
    public class BookService
    {
        private Respository<Book> _bookRepository;
        public BookService(Respository<Book> repository)
        {
            _bookRepository = repository;
        }

        // 目前只是简单地返回所有图书, 以后可以根据销量等指标进行排序和筛选
        // [2025/10/3] 增加了count参数, 用于指定获取的数量
        // [2025/10/3] 使用了OrderByDescending对图书进行排序, 以确保返回的图书是热销的
        // [2025/10/4] 重构方法, 采用了分页获取实体模型以及查询注入的思路
        /// <summary>
        /// 获取热销图书列表
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<BookViewModel>> GetPopularBooksAsync(int pageIndex = 1, int pageSize = 50)
        {
            // 获取书籍仓储的可查询对象
            var query = _bookRepository.AsQueryable();

            // 查询并按销量降序排序
            var popularQuery = query.OrderByDescending(b => b.Sales);

            // 分页获取热销书籍实体模型
            var popularBookEMs = await _bookRepository.GetPagedAsync(popularQuery, pageIndex, pageSize);

            // 转换为视图模型
            // 后续可能需要使用AutoMapper等工具进行转换,
            // 原本考虑视图模型需要访问多个仓储, 使用AutpMapper意义不大,
            // 但目前视图模型的属性都来自单一实体模型, 最好还是使用AutoMapper
            var popularBookVMs = popularBookEMs.Select(b => new BookViewModel()
            {
                Id = b.Id,
                Number = b.Number,
                Name = b.Name,
                Authors = b.Authors,
                Price = ((float)b.Price),
                Sales = b.Sales
            }).ToList();

            return popularBookVMs;
        }

        /// <summary>
        /// 获取所搜索图书列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<BookViewModel>> GetSearchedBooksAsync(string keyWord, int pageIndex = 1, int pageSize = 30)
        {
            throw new Exception("Not Implemented");
        }

        /// <summary>
        /// 根据书籍编号获取图书详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BookViewModel> GetBookInformationByNumberAsync(int Number)
        {
            var query = _bookRepository.AsQueryable();
            query = query.Where(b => b.Number == Number);

            var bookEM = await _bookRepository.GetSingleByQueryAsync(query);

            // TODO: 若果bookEM为null, 则抛出异常, 以后可以定义更具体的异常类型
            // 目前先用通用的Exception类型
            if (bookEM == null)
            {
                throw new Exception("Book Not Found");
            }

            // 转换为视图模型
            // TODO: AutoMapper
            var bookVM = new BookViewModel()
            {
                Id = bookEM.Id,
                Number = bookEM.Number,
                Name = bookEM.Name,
                Authors = bookEM.Authors,
                Publisher = bookEM.Publisher,
                PublishYear = bookEM.PublishYear,
                Category = bookEM.Categorys != null ? string.Join(", ", bookEM.Categorys) : null,
                Introduction = bookEM.Introduction,
                CoverImageUrl = bookEM.CoverImageUrl,
                Price = ((float)bookEM.Price),
                Sales = bookEM.Sales
            };

            return bookVM;
        }

        /// <summary>
        /// 根据书籍编号列表获取图书实体模型列表
        /// </summary>
        /// <param name="BookNumbers"></param>
        /// <returns></returns>
        public async Task<ServiceResult<List<Book>>> GetBookByNumberAsync(List<int> BookNumbers)
        {
            var booksQurey = _bookRepository.AsQueryable().Where(b => BookNumbers.Contains(b.Number));
            var books = await _bookRepository.GetListByQueryAsync(booksQurey);
            var ErrorMsg = string.Empty;

            foreach (var n in BookNumbers)
                if (books.Find(b => b.Number == n) is null)
                    ErrorMsg += $"编号{n}:书籍不存在\n";

            if (ErrorMsg != string.Empty)
                return ServiceResult<List<Book>>.Fail(ErrorMsg.Trim());

            return ServiceResult<List<Book>>.Success(books);
        }

        /// <summary>
        /// 根据图书编号获取单个图书实体模型
        /// </summary>
        /// <param name="BookNumbers"></param>
        /// <returns></returns>
        public async Task<ServiceResult<Book>> GetBookByNumberAsync(int BookNumber)
        {
            var booksQurey = _bookRepository.AsQueryable().Where(b => b.Number == BookNumber);
            var book = await _bookRepository.GetSingleByQueryAsync(booksQurey);

            if (book is null)
                return ServiceResult<Book>.Fail($"编号{BookNumber}:书籍不存在");

            return ServiceResult<Book>.Success(book);
        }
    }
}
