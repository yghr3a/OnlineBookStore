using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Repository;

namespace OnlineBookStore.Services
{
    // 图书服务, 提供与图书相关的业务逻辑
    public class BookService
    {
        private Repository<Book> _bookRepository;
        public BookService(Repository<Book> repository)
        {
            _bookRepository = repository;
        }

        // 目前只是简单地返回所有图书, 以后可以根据销量等指标进行排序和筛选
        // [2025/10/3] 增加了count参数, 用于指定获取的数量
        // [2025/10/3] 使用了OrderByDescending对图书进行排序, 以确保返回的图书是热销的
        /// <summary>
        /// 获取热销图书列表
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<BookViewModel>> GetPopularBooksAsync(int count = 30)
        {
            var books = await _bookRepository.GetAllAsync();

            // 将实体转换为视图模型
            var bookVMs = books.Select(b => new BookViewModel()
            {
                Id = b.Id,
                Name = b.Name,
                Author = b.Author,
                Number = b.Number,
                Price = b.Price,
                Sales = b.Sales
            }).ToList();

            // 根据销量排序
            var popularBookVMs = bookVMs.OrderByDescending(b => b.Sales).ToList();

            // 获取前count个热销图书
            var resultPopularBookVMs = popularBookVMs.GetRange(0, count);

            return resultPopularBookVMs;
        }

        /// <summary>
        /// 获取所搜索图书列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<BookViewModel>> GetSearchedBooksAsync(string keyWord, int pageIndex = 1, int pageSize = 30 )
        { 
            throw new Exception("Not Implemented");
        }

        /// <summary>
        /// 根据Id获取图书详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BookViewModel> GetBookInformationByIdAsync(int id)
        {
            throw new Exception("Not Implemented");
        }

    }
}
