using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;

namespace OnlineBookStore.Services
{
    public class BookFactory
    {
        public DataResult<BookViewModel> CreateBookViewModel(Book book)
        {
            var vm = new BookViewModel
            {
                Id = book.Id,
                Number = book.Number,
                Name = book.Name,
                Authors = book.Authors,
                Price = book.Price,
                Sales = book.Sales
            };
            return DataResult<BookViewModel>.Success(vm);
        }
    }
}
