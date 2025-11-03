using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Repository;
using System.ComponentModel;
using static OnlineBookStore.Infrastructure.ExceptionChecker;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 负责订单相关的业务操作
    /// </summary>
    public class OrderApplication

    {
        private OrderDomainService _orderDomainService;
        private UserDomainService _userDomainService;
        private BookDomainService _bookDomainService;
        private OrderFactory _orderFactory;

        public OrderApplication(OrderDomainService orderDomainService,
                            BookDomainService bookDomainService,
                            UserDomainService userDomainService,
                            OrderFactory orderFactor) 
        {
            _orderDomainService = orderDomainService;
            _userDomainService = userDomainService;
            _bookDomainService = bookDomainService;
            _orderFactory = orderFactor;
        }

        /// <summary>
        /// 下单操作业务方法
        /// </summary>
        /// [2025/11/03] 使用ExceptionChecker简化代码
        /// <param name="createOrderResponse"></param>
        /// <returns></returns>
        public async Task<InfoResult> PlayerOrderAsync(CreateOrderResponse createOrderResponse)
        {
            // 具体的支付API相关的调用就先不实现先
            // 还有购买完成后向用户发送下载链接的操作也不实现先
            // 目前只简单实现几点: 创建订单项, 创建订单, 添加进用户的历史订单里, 更新书籍的销量

            try
            {
                var user = await CheckAsync(_userDomainService.GetCurrentUserEntityModelAsync());
                var Numbers = createOrderResponse.Items.Select(i => i.BookNumber).ToList();
                var books = await CheckAsync(_bookDomainService.GetBookByNumberAsync(Numbers));

                var order = Check(_orderFactory.CreateOrderEntity(books!, user!, createOrderResponse));
                await CheckAsync(_orderDomainService.AddOrder(user!, order!, books!, createOrderResponse));

                return InfoResult.Success();
            }
            catch (Exception ex)
            {
                return InfoResult.Fail("创建订单失败 " +  ex.Message);
            }
        }

        /// <summary>
        /// 获取用户历史订单业务方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<DataResult<List<OrderViewModel>>> GetUserOrderAsync(int pageIndex = 1, int pageSize = 30)
        {
            try
            {
                var user = await CheckAsync(_userDomainService.GetCurrentUserEntityModelAsync());
                var orders = await CheckAsync(_orderDomainService.GetPagedOrdersByUserId(user!.Id, pageIndex, pageSize));

                var bookIds = orders.SelectMany(o => o.OrderItems)  // 多重select, 确保结果类型是List<int>, 而不是List<IEnumberable<int>>
                                    .Select(oi => oi.BookId)
                                    .Distinct()     // 去重
                                    .ToList();
                var books = await CheckAsync(_bookDomainService.GetBookByIdAsync(bookIds));

                var arge = new CreateOrderViewModelArge() { User = user, Books = books, Orders = orders };
                var orderVMs = Check(_orderFactory.CreateOrderViewModels(arge));

                return DataResult<List<OrderViewModel>>.Success(orderVMs);
            }
            catch (Exception ex)
            {
                return DataResult<List<OrderViewModel>>.Fail(ex.Message);
            }
        }

    }
}
