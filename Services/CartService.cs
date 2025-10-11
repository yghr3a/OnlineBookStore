using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 购物车服务, 负责处理与购物车相关的业务逻辑
    /// </summary>
    public class CartService
    {
        // 用户上下文信息
        private UserContext _userContext;

        private Respository<User> _userRespository;
        private Respository<Book> _bookRespository;
        private Respository<Cart> _cartRespository;

        // 在犹豫是否有必要定义一个User属性和Cart属性,直接通过UserContext获取用户Id, 然后通过AppDbContext获取用户和购物车对象似乎也挺方便的

        public CartService(UserContext userContext,
                           Respository<User> userRespository,
                           Respository<Book> bookRespository,
                           Respository<Cart> cartRespository)
        {
            _userContext = userContext;
            _userRespository = userRespository;
            _bookRespository = bookRespository;
            _cartRespository = cartRespository;
        }

        /// <summary>
        /// 将书籍添加到当前登录用户的购物车
        /// </summary>
        /// <param name="bookNumber"></param>
        /// <returns></returns>
        public async Task<CartAddResult> AddBookToUserCart(int bookNumber)
        {
            var userQuary = _userRespository.AsQueryable();
            var bookQuary = _bookRespository.AsQueryable();

            // TODO :发现UserContext里的UserId(用户编号)是string类型, 但是User实体类里的Number(用户编号)是int类型

            // 这里使用用户名作为索引 
            // [2025/10/11] 要使用Include和ThenInclude来加载相关的Cart和CartItems
            var user = await userQuary.Include(u => u.Cart)
                                      .ThenInclude(c => c.CartItems)
                                      .FirstOrDefaultAsync(u => u.UserName == _userContext.UserName);
            var book = await bookQuary.FirstOrDefaultAsync(b => b.Number == bookNumber);
            var cart = user?.Cart;

            if (user == null)
            {
                // 用户不存在, 可以抛出异常或返回错误信息
                return new CartAddResult()
                {
                    IsSuccess = false,
                    ErrorMsg = "用户不存在"
                };
            }

            if(book == null)
            {
                // 书籍不存在, 可以抛出异常或返回错误信息
                return new CartAddResult()
                {
                    IsSuccess = false,
                    ErrorMsg = "书籍不存在"
                };
            }

            if (cart == null)
            {
                // 如果用户没有购物车, 创建一个新的购物车
                // Cart与User一一对应, 无需手动给Cart.UserId赋值, EFCore会自动处理
                cart = new Cart();
                user.Cart = cart;

                // 保存数据, 因为此时cart里的UserID还没有值, 需要先保存User和Cart, 让cart.UserID有值
                await _cartRespository.SaveAsync();
                await _userRespository.SaveAsync();
            }

            var cartItem = new CartItem()
            {
                CartId = cart.UserId,
                BookId = book.Id,
                Count = 1,                 // 数量先设为一, 后续可以扩展为传入参数
                CreatedDate = DateTime.Now // 设置添加时间, 这里先简单处理
            };

            // 将书籍添加到购物车
            if (cart.CartItems == null)
            {
                cart.CartItems = new List<CartItem>();
            }
            cart.CartItems?.Add(cartItem);

            // 保存数据, EFCore会自动处理关联关系
            await _cartRespository.SaveAsync();

            return new CartAddResult() { IsSuccess = true };
        }
    }
}
