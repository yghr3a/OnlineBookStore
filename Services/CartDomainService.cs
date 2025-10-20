﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Pages;
using OnlineBookStore.Pages.Book;
using OnlineBookStore.Respository;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 购物车服务, 负责处理与购物车相关的业务逻辑
    /// </summary>
    public class CartDomainService
    {
        // 用户上下文信息
        private UserContext _userContext;

        private Respository<User> _userRespository;
        private Respository<Book> _bookRespository;
        private Respository<Cart> _cartRespository;

        // 在犹豫是否有必要定义一个User属性和Cart属性,直接通过UserContext获取用户Id, 然后通过AppDbContext获取用户和购物车对象似乎也挺方便的
        public CartDomainService(UserContext userContext,
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
        /// 获取当前登录用户的购物车
        /// [2025/10/20]思路有两种:1. 是依赖UserDomainService获取当前用户实例在获取其Cart属性, 但是默认ser的引导属性为空
        /// 2. 依赖UserContext和Repository<User>获取user,再通过引导属性获取Cart
        /// 3. 依赖UserContext和Repository<Cart>直接根据用户信息找到目标Cart
        /// 4. 依赖UserDomainService和Repository<Cart>获取当前用户user, 通过user.id间接获取Repository
        /// 采用方案4
        /// [2025/10/20] 重构, 该方法直接依赖AccountService其实是UserDomainService的过渡类, 所以这里其实犯了DDD架构一个常见错误:"依赖了另一个领域服务"的问题
        /// 所以重构为带一个int参数userId的方法, 由调用方负责获取当前用户Id
        /// </summary>
        /// <returns></returns>
        public async Task<DataResult<Cart>> GeCartByUserIdAsync(int userId)
        {
            // 根据userid获取user的cart, 记得引导属性CartItems得包含上
            var cartQuery = _cartRespository.AsQueryable()
                            .Include(c => c.CartItems)
                            .Where(c => c.UserId == userId);
            var cart = await _cartRespository.GetSingleByQueryAsync(cartQuery);

            if (cart is null)
                return DataResult<Cart>.Fail("找不到该用户的购物车数据");

            // 对于购物车项列表为空的情况的处理还有一种别的思路
            // 不确定会不会有数据库里面还有购物车项的数据, 但是就是莫名奇妙地没包含, 那么可以重新查询再加回去
            // 默认情况下cart.CartItem是自己会new一个的
            // 除了返回错误信息, 也可以直接给CartItems直接new一个对象
            if(cart.CartItems is null)
                cart.CartItems = new List<CartItem>();

            return DataResult<Cart>.Success(cart);
        }

        /// <summary>
        /// 将图书添加到指定购物车
        /// </summary>
        /// <param name="book"></param>
        /// <param name="cart"></param>
        /// <returns></returns>
        public async Task<InfoResult> AddBookToCartAsync(Book book, Cart cart)
        {
            // [2025/10/20] 这部分的创建应该交给一个工厂类来做, 这里先简单处理
            var cartItem = new CartItem()
            {
                CartId = cart!.UserId,      // CardId就是UserId
                BookId = book!.Id,
                Count = 1,                 // 数量先设为一, 后续可以扩展为传入参数
                CreatedDate = DateTime.Now // 设置添加时间, 这里先简单处理
            };

            // [2025/10/17] 出现了一个bug, 重复添加同一本书籍时会出现多条"同种书籍, 购买数为一"的记录, 需要先检查是否已经存在该书籍的CartItem
            var exitItem = cart.CartItems.Find(i => i.BookId == book.Id);
            if (exitItem is null)
                cart.CartItems?.Add(cartItem);  // 若不存在则添加新的CartItem
            else
                exitItem.Count += 1; // 已存在则数量加一

            // 保存数据, EFCore会自动处理关联关系
            await _cartRespository.SaveAsync();

            return InfoResult.Success();
        }
    }
}
