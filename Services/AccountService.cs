using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;

namespace OnlineBookStore.Services
{
    public class AccountService
    {
        private Respository<User> _responsity;

        public AccountService(Respository<User> repository)
        {
            _responsity = repository;
        }
    }
}
