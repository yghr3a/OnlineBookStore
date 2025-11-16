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
        private MockPaymentGateway _mockPaymentGateway;
        private OrderFactory _orderFactory;

        public OrderApplication(OrderDomainService orderDomainService,
                            BookDomainService bookDomainService,
                            UserDomainService userDomainService,
                            MockPaymentGateway mockPaymentGateway,
                            OrderFactory orderFactor)
        {
            _orderDomainService = orderDomainService;
            _userDomainService = userDomainService;
            _bookDomainService = bookDomainService;
            _mockPaymentGateway = mockPaymentGateway;
            _orderFactory = orderFactor;
        }

        /// <summary>
        /// 下单操作业务方法
        /// </summary>
        /// [2025/11/03] 使用ExceptionChecker简化代码
        /// <param name="createOrderResponse"></param>
        /// <returns></returns>
        public async Task<DataResult<PlaceOrderPaymentResponse>> PlaceOrderAsync(CreateOrderResponse createOrderResponse)
        {
            // 具体的支付API相关的调用就先不实现先
            // 还有购买完成后向用户发送下载链接的操作也不实现先
            // 目前只简单实现几点: 创建订单项, 创建订单, 添加进用户的历史订单里, 更新书籍的销量

            try
            {
                // ---------- 创建订单相关操作 -------------
                var user = await CheckAsync(_userDomainService.GetCurrentUserEntityModelAsync());
                var Numbers = createOrderResponse.Items.Select(i => i.BookNumber).ToList();
                var books = await CheckAsync(_bookDomainService.GetBookByNumberAsync(Numbers));

                var order = Check(_orderFactory.CreateOrderEntity(books!, user!, createOrderResponse));
                await CheckAsync(_orderDomainService.AddOrderAsync(user!, order!, books!, createOrderResponse));

                // ---------- 调用第三方（Mock）统一下单 -------------
                var mockPaymentQr = await CheckAsync(_mockPaymentGateway.CreatePaymentAsync(order!));

                var res = new PlaceOrderPaymentResponse()
                {
                    CodeUrl = mockPaymentQr.CodeUrl,           // 最重要的支付二维码
                    TransactionId = mockPaymentQr.OrderNumber, // 暂时使用订单号代替, 反正是模拟的
                    ExpireAt = mockPaymentQr.ExpireAt          // 二维码过期时间
                };

                return DataResult<PlaceOrderPaymentResponse>.Success(res);
            }
            // TODO: 注意一下, 如果订单创建成功, 但是支付调用失败的话, 目前的实现是不会回滚订单创建的
            // 为了契合ExceptCheck的设计方式, 可以考虑顶一个对该情况专门的业务异常类, 在这里捕获后, 手动回滚订单创建
            catch (Exception ex)
            {
                return DataResult<PlaceOrderPaymentResponse>.Fail("创建订单失败 " + ex.Message);
            }
        }

        /// <summary>
        /// 重新下单业务方法
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public async Task<DataResult<PlaceOrderPaymentResponse>> ReplaceOrderAsync(int orderNumber)
        {
            try
            {
                // ---------- 获取订单相关操作 -------------
                var order = await CheckAsync(_orderDomainService.GetOrderByOrderNumber(orderNumber));
                // 让订单重新变为待支付状态
                order.OrderStatus = OrderStatus.WaitingForPayment;
                await CheckAsync(_orderDomainService.UpdateAsync(order!));

                // ---------- 调用第三方（Mock）统一下单 -------------
                var mockPaymentQr = await CheckAsync(_mockPaymentGateway.CreatePaymentAsync(order!));

                var res = new PlaceOrderPaymentResponse()
                {
                    CodeUrl = mockPaymentQr.CodeUrl,           // 最重要的支付二维码
                    TransactionId = mockPaymentQr.OrderNumber, // 暂时使用订单号代替, 反正是模拟的
                    ExpireAt = mockPaymentQr.ExpireAt          // 二维码过期时间
                };

                return DataResult<PlaceOrderPaymentResponse>.Success(res);
            }
            catch (Exception ex)
            {
                return DataResult<PlaceOrderPaymentResponse>.Fail("重新下单失败 " + ex.Message);
            }
        }


        /// <summary>
        /// 获取用户历史订单业务方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<DataResult<List<OrderViewModel>>> GetUserOrderAsync(int pageIndex = 1, int pageSize = 10)
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

                // 实现分页, 后续考虑下放
                orderVMs.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return DataResult<List<OrderViewModel>>.Success(orderVMs);
            }
            catch (Exception ex)
            {
                return DataResult<List<OrderViewModel>>.Fail(ex.Message);
            }
        }

        public async Task<DataResult<List<OrderViewModel>>> GetUserSearchedOrderAsync(string keyword, int pageIndex = 1, int pageSize = 10)
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

                // 实现关键字查询, 后续考虑下放
                var qurey = orderVMs.AsQueryable();
                if (string.IsNullOrEmpty(keyword) == false)
                    qurey = qurey.Where(orderVM => orderVM.OrderItemViewModels
                        .Any(item => item.BookTitle.Contains(keyword, StringComparison.OrdinalIgnoreCase)));

                // 实现分页, 后续考虑下放
                orderVMs = qurey.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return DataResult<List<OrderViewModel>>.Success(orderVMs);
            }
            catch (Exception ex)
            {
                return DataResult<List<OrderViewModel>>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 根据订单编号获取订单状态业务方法
        /// </summary>
        /// <param name="OrderNumber"></param>
        /// <returns></returns>
        public async Task<DataResult<OrderStatus>> CheckOrderStatusByOrderNumber(int OrderNumber)
        {
            try
            {
                var order = await CheckAsync(_orderDomainService.GetOrderByOrderNumber(OrderNumber));
                return DataResult<OrderStatus>.Success(order!.OrderStatus);
            }
            catch (Exception ex)
            {
                return DataResult<OrderStatus>.Fail("获取订单状态失败 " + ex.Message);
            }

        }

        /// <summary>
        /// 更新订单支付状态业务方法
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        public async Task<InfoResult> UpdateOrderPaymentStatusAsync(int orderNumber, OrderStatus orderStatus)
        {
            try
            {
                var order = await CheckAsync(_orderDomainService.GetOrderByOrderNumber(orderNumber));
                order.OrderStatus = orderStatus;
                await CheckAsync(_orderDomainService.UpdateAsync(order));
                return InfoResult.Success();
            }
            catch (Exception ex)
            {
                return InfoResult.Fail("更新订单支付状态失败 " + ex.Message);
            }
        }

        /// <summary>
        /// 更新订单支付状态业务方法
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        public async Task<InfoResult> UpdateOrderPaymentStatusAsync(string orderNumber, OrderStatus orderStatus)
        {
            try
            {
                var orderNumberNum = int.Parse(orderNumber);
                return await UpdateOrderPaymentStatusAsync(orderNumberNum, orderStatus);    
            }
            catch (Exception ex)
            {
                return InfoResult.Fail("更新订单支付状态失败 " + ex.Message);
            }
        }
    }
}
