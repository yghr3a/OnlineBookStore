using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;

namespace OnlineBookStore.Services
{
    public class BookFactory
    {
        private NumberFactory _numberFactory;

        public BookFactory(NumberFactory numberFactory)
        {
            _numberFactory = numberFactory;
        }

        /// <summary>
        /// 书籍视图模型的创建
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public DataResult<BookViewModel> CreateBookViewModel(Book book)
        {
            var vm = new BookViewModel
            {
                Id = book.Id,
                Number = book.Number,
                Status = book.Status,
                Name = book.Name,
                Authors = book.Authors,
                Price = book.Price,
                Sales = book.Sales
            };
            return DataResult<BookViewModel>.Success(vm);
        }

        /// <summary>
        /// 书籍实体模型的创建
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public DataResult<Book> CreateBook(CreateBookResponse response)
        {
            var number = _numberFactory.CreateNumber<Book>();

            var book = new Book
            {
                Number = number,
                Status = response.Status,
                Name = response.Name,
                Authors = response.Authors,
                Price = response.Price,
                Sales = response.Sales,

                CoverImageUrl = response.CoverImageUrl,
                Introduction = response.Introduction
            };

            return DataResult<Book>.Success(book);
        }
    }
}
