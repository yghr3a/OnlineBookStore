using OnlineBookStore.Respository;
using OnlineBookStore.Models;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 订单邻域服务, 只负责纯粹的单独业务
    /// </summary>
    public class OrderDomainService
    {
        private UnitOfWork _unitOfWork;
        private Respository<Book> _bookRespository;
        private Respository<Order> _orderRespository;

        public OrderDomainService(UnitOfWork unitOfWork, 
                                  Respository<Book> bookRespository, 
                                  Respository<Order> orderRespository)
        {
            _unitOfWork = unitOfWork;
            _bookRespository = bookRespository;
            _orderRespository= orderRespository;
        }
        
        /// <summary>
        /// 添加订单, 更新书籍销量, 更新用户历史订单
        /// [2025/10/16] 错误情况处理先简单实现, 后续使用业务异常来处理更好
        /// </summary>  
        public async Task<InfoResult> AddOrder(User user, Order order, List<Book> books, CreateOrderResponse response)
        {
            if (user is null) return InfoResult.Fail("用户信息为空");
            if (order is null) return InfoResult.Fail("订单信息为空");
            if (books is null) return InfoResult.Fail("书籍信息为空");
            if (books.Count != response.Items.Count) return InfoResult.Fail("部分书籍不存在");

            // 建立字典
            var BookNumber2Count = response.Items.ToDictionary(i => i.BookNumber, i => i.Count);

            await _unitOfWork.ExecuteInTransactionAsync( async () =>
            {
                // 添加订单到数据库里
                await _orderRespository.AddAsync(order);

                // 更新各个书籍的销量
                foreach (var book in books)
                {
                    book.Sales += BookNumber2Count[book.Number];
                    _bookRespository.Update(book);
                }

                // 更新用户历史订单
                user.Orders.Add(order);
            });

            return InfoResult.Success();
        }

        /// <summary>
        /// 根据用户Id获取该用户的所有订单
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<DataResult<List<Order>>> GetAllOrdersByUserId(int userId)
        {
            var orderQuery = _orderRespository.AsQueryable()
                                              .Include(o => o.OrderItems)
                                              .Where(o => o.UserId == userId);
            var orders = await _orderRespository.GetListByQueryAsync(orderQuery);

            // 不知道会不会由orders为null的情况

            return DataResult<List<Order>>.Success(orders);
        }

        /// <summary>
        /// 根据用户Id获取该用户的分页订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<DataResult<List<Order>>> GetPagedOrdersByUserId(int userId, int pageIndex = 1, int pageSize = 30)
        {
            var orderQuery = _orderRespository.AsQueryable()
                                               .Include(o => o.OrderItems)
                                               .Where(o => o.UserId == userId);
            var pageOrders = await _orderRespository.GetPagedAsync(orderQuery,pageIndex, pageSize);

            // 不知道会不会由orders为null的情况

            return DataResult<List<Order>>.Success(pageOrders);
        }
    }
}
