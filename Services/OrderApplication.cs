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
        private OrderDomainService _orderDomainService;
        private AccountService _accountService;
        private BookDomainService _bookDomainService;
        private OrderFactory _orderFactory;

        public OrderApplication(OrderDomainService orderDomainService,
                            BookDomainService bookDomainService,
                            AccountService accountService,
                            OrderFactory orderFactor) 
        {
            _orderDomainService = orderDomainService;
            _accountService = accountService;
            _bookDomainService = bookDomainService;
            _orderFactory = orderFactor;
        }

        /// <summary>
        /// 创建订单业务方法
        /// </summary>
        /// <param name="createOrderResponse"></param>
        /// <returns></returns>
        public async Task<CreateOrderResult> PlayerOrderAsync(CreateOrderResponse createOrderResponse)
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

            // 获取书籍信息
            // [2025/10/16] 将获取书籍信息的工作交给了BookDomainService了
            var booksRes = await _bookDomainService.GetBookByNumberAsync(Numbers);
            var books = booksRes.Data;
            if (booksRes.IsSuccess == false)
                return new CreateOrderResult() { IsSuccessed = false, ErrorMsg = booksRes.ErrorMsg };

            // [2025/10/16]使用OrderFactory创建订单
            var orderReuslt = _orderFactory.Create(books!, user!, createOrderResponse);
            var order = orderReuslt.Data;
            if(orderReuslt.IsSuccess == false)
                return new CreateOrderResult() { IsSuccessed = false, ErrorMsg = userRes.ErrorMsg };

            // [2025/10/16] 使用OrderDomainService完成添加订单的业务
            var result = await _orderDomainService.AddOrder(user!, order!, books!, createOrderResponse);
            if(result.IsSuccess == false)
                return new CreateOrderResult() { IsSuccessed = true };

            return new CreateOrderResult() { IsSuccessed = true };
        }
    }
}
