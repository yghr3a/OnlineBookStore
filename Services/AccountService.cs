using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;

namespace OnlineBookStore.Services
{
    public class AccountService
    {
        private Repository<User> _responsity;

        public AccountService(Repository<User> repository)
        {
            _responsity = repository;
        }
    }
}
