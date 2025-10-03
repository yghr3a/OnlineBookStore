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
        /// <summary>
        /// 获取热销图书列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<BookViewModel>> GetPopularBooksAsync(int count = 30)
        {
            var books = await _bookRepository.GetAllAsync();

            var bookVMs = books.Select(b => new BookViewModel()
            {
                Name = b.Name,
                Author = b.Author,
                Number = b.Number,
                //Price = 100,         //  TODO: 这里应该是从另一个仓储获取价格
                Sales = 100            // TODO: 这里应该是从另一个仓储获取销量
            }).ToList().GetRange(0, count);

            return bookVMs;
        }

        /// <summary>
        /// 获取所搜索图书列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<BookViewModel>> GetSearchedBooksAsync()
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
