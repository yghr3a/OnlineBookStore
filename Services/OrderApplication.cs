using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;
using System.ComponentModel;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 负责订单相关的业务操作
    /// </summary>
    public class OrderApplication

    {
        private UserContext _userContext;
        private UnitOfWork _unitOfWork;

        private Respository<Book> _bookRespository;
        private Respository<Order> _orderRepository;
        private Respository<Cart> _cartRespository;

        private AccountService _accountService;
        private BookService _bookService;
        private OrderFactory _orderFactory;

        public OrderApplication(UserContext userContext,
                            UnitOfWork unitOfWork,
                            Respository<Book> bookRespository,
                            Respository<Order> orderRepository,
                            Respository<Cart> cartRespository,
                            BookService bookService,
                            AccountService accountService,
                            OrderFactory orderFactor) 
        {
            _userContext = userContext;
            _unitOfWork = unitOfWork;

            _bookRespository = bookRespository;
            _orderRepository = orderRepository;
            _cartRespository = cartRespository;

            _accountService = accountService;
            _bookService = bookService;
            _orderFactory = orderFactor;
        }

        /// <summary>
        /// 创建订单业务方法
        /// </summary>
        /// <param name="createOrderResponse"></param>
        /// <returns></returns>
        public async Task<CreateOrderResult> CreateOrderAsync(CreateOrderResponse createOrderResponse)
        {
            // 具体的支付API相关的调用就先不实现先

            // 还有购买完成后向用户发送下载链接的操作也不实现先

            // 目前只简单实现几点: 创建订单项, 创建订单, 添加进用户的历史订单里, 更新书籍的销量

            // 首先先验证用户, 书籍等信息是否存在
            // [2025/10/15] 将获取用户信息的工作交给AccountService负责了
            var userRes = await _accountService.GetCurrentUserEntityModelAsync();
            var user = userRes.Data;
            if (userRes.IsSuccess == false)
                return new CreateOrderResult() { IsSuccessed = false, ErrorMsg = userRes.ErrorMsg };

            var Numbers = createOrderResponse.Items.Select(i => i.BookNumber).ToList();

            var booksRes = await _bookService.GetBookByNumberAsync(Numbers);
            var books = booksRes.Data;
            if (booksRes.IsSuccess == false)
                return new CreateOrderResult() { IsSuccessed = false, ErrorMsg = booksRes.ErrorMsg };

            var orderReuslt = _orderFactory.Create(books!, user!, createOrderResponse);
            if(orderReuslt.IsSuccess == false)
                return new CreateOrderResult() { IsSuccessed = false, ErrorMsg = userRes.ErrorMsg };
            var order = orderReuslt.Data;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // 添加订单
                await _orderRepository.AddAsync(order!);

                // 更新书籍销量
                foreach (var b in books!)
                {
                    b.Sales += BookNumber2Count[b.Number];
                    _bookRespository.Update(b);
                }

                // 更新用户的历史订单
                user.Orders.Add(order);
            });

            return new CreateOrderResult() { IsSuccessed = true };
        }
    }
}
