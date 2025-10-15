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
    public class OrderService
    {
        private UserContext _userContext;
        private UnitOfWork _unitOfWork;

        private Respository<Book> _bookRepository;
        private Respository<Order> _orderRepository;
        private Respository<Cart> _cartRespository;
        private Respository<User> _userRespository;

        public OrderService(UserContext userContext,
                            UnitOfWork unitOfWork,
                            Respository<Book> bookRepository,
                            Respository<Order> orderRepository,
                            Respository<Cart> cartRespository,
                            Respository<User> userRepository) 
        {
            _userContext = userContext;
            _unitOfWork = unitOfWork;
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
            _cartRespository = cartRespository;
            _userRespository = userRepository;
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
            var userQuery = _userRespository.AsQueryable()
                            .Where(u => u.UserName == _userContext.UserName);

            var user = await _userRespository.GetSingleByQueryAsync(userQuery);

            if (user == null)
                return new CreateOrderResult() { IsSuccessed = false, ErrorMsg = "用户不存在" };

            var Numbers = createOrderResponse.Items.Select(i => i.BookNumber).ToList();
            var booksQurey = _bookRepository.AsQueryable().Where(b => Numbers.Contains(b.Number));
            var books = await _bookRepository.GetListByQueryAsync(booksQurey);

            // 数量对不上, 说明部分书籍不存在
            if (books.Count != createOrderResponse.Items.Count)
                return new CreateOrderResult() { IsSuccessed = false, ErrorMsg = "部分书籍不存在" };

            // 建立字典, 将购买数量与bookNumber绑定
            var BookNumber2Count = createOrderResponse.Items.ToDictionary(i => i.BookNumber, i => i.Count);

            // 开始创建订单项, 创建订单, 添加进用户的历史订单里, 更新书籍的销量
            var orderItems = books.Select(b => new OrderItem
            {
                BookId = b.Id,
                Price = b.Price,
                Count = BookNumber2Count[b.Number]      // 使用字典高效
            }).ToList();

            var order = new Order
            {
                // Number字段后续需要改为更合理的生成方式, 现在只是简单的用时间戳
                Number = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % int.MaxValue),

                UserId = user.Id,
                User = user,
                PaymentMethod = createOrderResponse.PaymentMethod,
                OrderState = createOrderResponse.OrderState,
                CreatedDate = DateTime.UtcNow,
                OrderItems = orderItems
            };

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // 添加订单
                await _orderRepository.AddAsync(order);

                // 更新书籍销量
                foreach (var b in books)
                {
                    b.Sales += BookNumber2Count[b.Number];
                    _bookRepository.Update(b);
                }

                // 更新用户的历史订单
                user.Orders.Add(order);
            });

            return new CreateOrderResult() { IsSuccessed = true };
        }
    }
}
