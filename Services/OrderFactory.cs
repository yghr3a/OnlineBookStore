using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using Org.BouncyCastle.Asn1.X509;
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
        
        /// <summary>
        /// 创建订单视图模型方法
        /// [2025/10/22] 这里选择传入book字典是不想重复建立字典, 后续在优化
        /// </summary>
        /// <param name="order"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        public DataResult<List<OrderItemViewModel>> CreateOrderItemModels(Order order, Dictionary<int, Book> bookId2Book)
        {
            try
            {
                var orderItemVMs = order.OrderItems.Select(item => {
                    var book = bookId2Book[item.BookId];
                    return new OrderItemViewModel
                    {
                        Number = item.Number,
                        OrderNumber = order.Number,
                        BookNumber = book.Number,

                        BookTitle = book.Name,
                        BookCoverImageUrl = book.CoverImageUrl ?? string.Empty,
                        Price = (decimal)item.Price!,
                        Count = item.Count,
                    };
                }).ToList();

                return DataResult<List<OrderItemViewModel>>.Success(orderItemVMs);
            }
            catch (KeyNotFoundException)
            {
                // 加个异常捕获, 以防万一字典中没有对应的数据

                // 这里的处理选择直接向上抛出
                throw;
            }
        }

        /// <summary>
        /// 创建订单视图模型方法
        /// </summary>
        /// <param name="arge"></param>
        /// <returns></returns>
        public DataResult<List<OrderViewModel>> CreateOrderViewModels(CreateOrderViewModelArge arge)
        {
            var user = arge.User;
            var orders = arge.Orders;
            var books = arge.Books;

            // 建立字典
            var bookId2Book = books.ToDictionary(b => b.Id, b => b);

            try
            {
                var orderVMsRes = orders.Select(order =>
                    new OrderViewModel{
                        Number = order.Number,
                        UserNumber = user.Number,

                        OrderStatus = order.OrderState,
                        PaymentMethod = order.PaymentMethod,
                        CreatedDate = order.CreatedDate,

                        // 这里没有做判断, 靠的是CreateOrderItemModels方法中的异常抛出, 之后被下面的catch捕获并返回DataResult
                        OrderItemViewModels =  CreateOrderItemModels(order, bookId2Book).Data! 
                    }).ToList();

                return DataResult<List<OrderViewModel>>.Success(orderVMsRes);
            }
            catch (KeyNotFoundException)
            {
                // 加个异常捕获, 以防万一字典中没有对应的数据
                // TODO: 记得将创建orderItemVMs的部分抽离成一个独立的方法
                return DataResult<List<OrderViewModel>>.Fail("未找到订单项所对应的书籍");
            }
        }
    }
}
