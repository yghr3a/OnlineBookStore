using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Repository;
using OnlineBookStore.Infrastructure;
using static OnlineBookStore.Infrastructure.ExceptionChecker;
using Microsoft.VisualBasic;

namespace OnlineBookStore.Services
{
    // 图书服务, 提供与图书相关的业务逻辑
    public class BookApplication
    {
        private Repository<Book> _bookRepository;
        private BookDomainService _bookDomainService;
        public BookApplication(BookDomainService bookDomainService,Repository<Book> repository)
        {
            _bookRepository = repository;
            _bookDomainService = bookDomainService;
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
                Price = b.Price,
                Sales = b.Sales
            }).ToList();

            return popularBookVMs;
        }

        /// <summary>
        /// 获取所搜索图书列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<DataResult<List<BookViewModel>>> GetSearchedBooksAsync(string keyWord, int pageIndex = 1, int pageSize = 50)
        {
            try
            {
                var books = await CheckAsync(_bookDomainService.GetSearchedBooksByKeywordAync(keyWord, pageIndex, pageSize));
                var bookVMs = books.Select(b => new BookViewModel()
                {
                    Id = b.Id,
                    Number = b.Number,
                    Name = b.Name,
                    Authors = b.Authors,
                    Price = b.Price,
                    Sales = b.Sales
                }).ToList();

                return DataResult<List<BookViewModel>>.Success(bookVMs);
            }
            catch (Exception ex)
            {
                return DataResult<List<BookViewModel>>.Fail("搜索书籍失败" + ex.Message);
            }
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
                Category = bookEM.Categorys,
                Introduction = bookEM.Introduction,
                CoverImageUrl = bookEM.CoverImageUrl,
                Price = bookEM.Price,
                Sales = bookEM.Sales
            };

            return bookVM;
        }
    }
}
