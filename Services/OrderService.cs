using Microsoft.AspNetCore.DataProtection.Repositories;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 负责订单相关的业务操作
    /// </summary>
    public class OrderService
    {
        private Respository<Book> _bookRepository;
        private Respository<Order> _orderRepository;
        private Respository<Cart> _cartRespository;
        private UserContext _userContext;

        public OrderService(UserContext userContext,
                            Respository<Book> bookRepository,
                            Respository<Order> orderRepository,
                            Respository<Cart> cartRespository) 
        {
            _userContext = userContext;
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
            _cartRespository = cartRespository;
        }

        public async Task CreatOrder()
        {

        }
    }
}
