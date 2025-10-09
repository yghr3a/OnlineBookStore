using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;
using System.Security.Claims;

namespace OnlineBookStore.Services
{
    public class AccountService
    {
        private Respository<User> _responsity;
        private UserContext _userContext;

        public AccountService(Respository<User> repository, UserContext userContext)
        {
            _responsity = repository;
            _userContext = userContext;
        }

        public async Task<bool> VerifyLoggedInUserInformation(string userName, string password)
        {
            return true;
        }
    }
}
