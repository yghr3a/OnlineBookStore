using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Repository;

namespace OnlineBookStore.Services
{
    public class BookService
    {
        private Repository<Book> _bookRepository;
        public BookService(Repository<Book> repository) 
        {
            _bookRepository = repository;
        }

        public async List<BookViewModel> GetPopularBooks()
        {
            var books = await _bookRepository.GetAllAsync();

            books.Select(b => new BookViewModel()
            {
                Name = b.Name,
                Author = b.Author,
                Number = b.Number
            }).ToList();
        }
    }
}
