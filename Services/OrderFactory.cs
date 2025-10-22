using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 订单工厂, 专门负责创建没有问题的Order对象
    /// </summary>
    public class OrderFactory
    {
        /// <summary>
        /// 订单创建方法, 一定要确保参数不会空, 以及books和response一一对应
        /// </summary>
        /// <param name="books"></param>
        /// <param name="user"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public DataResult<Order> CreateOrderEntity(List<Book> books, User user, CreateOrderResponse response)
        {
            // 建立字典, 将购买数量与bookNumber绑定
            var BookNumber2Count = response.Items.ToDictionary(i => i.BookNumber, i => i.Count);

            // 开始创建订单项, 创建订单, 添加进用户的历史订单里, 更新书籍的销量
            var orderItems = books!.Select(b => new OrderItem
            {
                BookId = b.Id,
                Price = b.Price,
                Count = BookNumber2Count[b.Number]      // 使用字典高效
            }).ToList();

            var order = new Order
            {
                // Number字段后续需要改为更合理的生成方式, 现在只是简单的用时间戳
                Number = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % int.MaxValue),

                UserId = user!.Id,
                User = user,
                PaymentMethod = response.PaymentMethod,
                OrderState = response.OrderState,
                CreatedDate = DateTime.UtcNow,
                OrderItems = orderItems
            };

            return DataResult<Order>.Success(order);
        }

        public DataResult<List<OrderViewModel>> CreateOrderViewModels(CreateOrderViewModelArge arge)
        {
            var user = arge.User;
            var orders = arge.Orders;
            var books = arge.Books;

            // 建立字典
            var bookId2Book = books.ToDictionary(b => b.Id, b => b);
            var orderId2Order = orders.ToDictionary(o => o.Id, o => o);

            try
            {
                var orderVMsRes = new List<OrderViewModel>();
                foreach (var order in orders)
                {
                    var itemVMs = order.OrderItems.Select(item => {
                        var book = bookId2Book[item.BookId];
                        return new OrderItemViewModel
                        {
                            Number = item.Number,
                            OrderNumber = orderId2Order[item.OrderId].Number,
                            BookNumber = book.Number,

                            BookTitle = book.Name,
                            BookCoverImageUrl = book.CoverImageUrl ?? string.Empty,
                            Price = (decimal)item.Price!,
                            Count = item.Count,
                        };
                    }).ToList();

                    var orderVM = new OrderViewModel
                    {
                        Number = order.Number,
                        UserNumber = user.Number,

                        OrderState = order.OrderState,
                        PaymentMethod = order.PaymentMethod,
                        CreatedDate = order.CreatedDate,

                        OrderItemViewModels = itemVMs
                    };

                    orderVMsRes.Add(orderVM);
                }

                return DataResult<List<OrderViewModel>>.Success(orderVMsRes);
            }
            catch (KeyNotFoundException ex)
            {
                // 加个异常捕获, 以防万一字典中没有对应的数据
                // TODO: 记得将创建orderItemVMs的部分抽离成一个独立的方法
                return DataResult<List<OrderViewModel>>.Fail(": " + ex.Message);
            }
        }
    }
}
