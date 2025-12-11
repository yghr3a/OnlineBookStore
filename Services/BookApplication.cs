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
        private BookDomainService _bookDomainService;
        private BookFactory _bookFactory;
        public BookApplication(BookDomainService bookDomainService,BookFactory bookFactory)
        {
            _bookDomainService = bookDomainService;
            _bookFactory = bookFactory;
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
        public async Task<DataResult<List<BookViewModel>>> GetPopularBooksAsync(int pageIndex = 1, int pageSize = 50)
        {
            try
            {
                var books = await CheckAsync(_bookDomainService.GeGetPopularBooksAsync(pageIndex, pageSize));
                // 使用 Task.WhenAll 将每个 book 的处理并发化
                var bookVMTasks = books.Select(book => Task.Run(() => Check(_bookFactory.CreateBookViewModel(book))));
                // 等待所有异步任务执行完毕
                var bookVM = await Task.WhenAll(bookVMTasks);
                // 转换结果为 List<BookViewModel>
                return DataResult<List<BookViewModel>>.Success(bookVM.ToList());

            }
            catch (Exception ex)
            {
                return DataResult<List<BookViewModel>>.Fail("获取热销书籍失败" + ex.Message);
            }
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
                // 使用 Task.WhenAll 将每个 book 的处理并发化
                var bookVMTasks = books.Select(book => Task.Run(() => Check(_bookFactory.CreateBookViewModel(book))));
                // 等待所有异步任务执行完毕
                var bookVM = await Task.WhenAll(bookVMTasks);
                // 转换结果为 List<BookViewModel>
                return DataResult<List<BookViewModel>>.Success(bookVM.ToList());
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
        public async Task<DataResult<BookViewModel>> GetBookInformationByNumberAsync(int Number)
        {
            try
            {
                var book = await CheckAsync(_bookDomainService.GetBookByNumberAsync(Number));
                var bookVM = Check(_bookFactory.CreateBookViewModel(book));
                return DataResult<BookViewModel>.Success(bookVM);
            }
            catch (Exception ex)
            {
                return DataResult<BookViewModel>.Fail("获取书籍详情失败" + ex.Message);
            }
        }
    }
}
