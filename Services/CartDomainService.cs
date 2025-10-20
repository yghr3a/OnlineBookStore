using Microsoft.EntityFrameworkCore;
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

            if (book == null)
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

            // [2025/10/17] 出现了一个bug, 重复添加同一本书籍时会出现多条"同种书籍, 购买数为一"的记录, 需要先检查是否已经存在该书籍的CartItem
            var exitItem = cart.CartItems.Find(i => i.BookId == book.Id);
            if (exitItem is null)
                cart.CartItems?.Add(cartItem);  // 若不存在则添加新的CartItem
            else
                exitItem.Count += 1; // 已存在则数量加一

            // 保存数据, EFCore会自动处理关联关系
            await _cartRespository.SaveAsync();

            return new CartAddResult() { IsSuccess = true };
        }

        /// <summary>
        /// 获取用户的购物车
        /// </summary>
        public async Task<GetCartResult> GetUserCartAsync(int pageIndex = 1, int pageSize = 30)
        {
            var userQuary = _userRespository.AsQueryable();

            var quary = userQuary.Include(u => u.Cart)
                          .ThenInclude(c => c.CartItems)
                          .ThenInclude(ci => ci.Book)
                          .Where(u => u.UserName == _userContext.UserName);
            // 因为UserContext的用户编号是字符串类型, 但是users里的编号是int类型,所以使用用户名作为索引

            // [2025/10/13] 这里使用GetPagedAsync方法来获取用户, 避免一次性加载过多数据
            // 这里获取的类型虽然是List<User>, 但实际上只会有一个用户
            var users = await _userRespository.GetPagedAsync(quary, pageIndex, pageSize);
            var user = users.FirstOrDefault();

            // 用户不存在
            if (user == null)
            {
                return new GetCartResult()
                {
                    IsSuccess = false,
                    ErrorMsg = "用户不存在"
                };
            }

            // 若没有购物车，则直接返回空模型（这里不写入数据库）
            var cart = user.Cart ?? new Cart { CartItems = new List<CartItem>() };

            // 将Cart转换为CartViewModel
            var cartItemVMs = cart.CartItems?.Select(ci => new CartItemViewModel
            {
                Number = ci.Id, // 先使用Id作为唯一标识, 忘记给CartItem添加Number属性了
                BookNumber = ci.Book?.Number ?? 0,
                BookTitle = ci.Book?.Name ?? "未知书籍",
                BookCoverImageUrl = ci.Book?.CoverImageUrl ?? string.Empty,
                BookAuthor = ci.Book?.Authors?.FirstOrDefault() ?? "未知作者",
                Count = ci.Count,
                AddedDate = ci.CreatedDate,
                Price = (float)(ci.Book?.Price ?? 0),
            }).ToList();

            // 构建CartViewModel
            var cartVM = new CartViewModel()
            {
                UserNumber = user.Number,
                CartItemViewModels = cartItemVMs ?? new List<CartItemViewModel>()
            };

            // 返回结果
            return new GetCartResult()
            {
                IsSuccess = true,
                CartViewModel = cartVM
            };
        }


        /// <summary>
        /// 移除用户购物车中的单个项
        /// </summary>
        /// <param name="bookNumber"></param>
        /// <returns></returns>
        public async Task<InfoResult> RemoveUserCartSingleItemAsync(int orderItemNumber)
        {
            var userQuary = _userRespository.AsQueryable();

            var quary = userQuary.Include(u => u.Cart)
                          .ThenInclude(c => c.CartItems)
                          .ThenInclude(ci => ci.Book)
                          .Where(u => u.UserName == _userContext.UserName);

            var user = await _userRespository.GetSingleByQueryAsync(quary);

            // 用户不存在
            if (user == null)
                return InfoResult.Fail("用户不存在");

            var cart = user.Cart;
            if (cart == null || cart.CartItems == null)
                return InfoResult.Fail("购物车为空");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == orderItemNumber);
            if (cartItem == null)
                return InfoResult.Fail("购物车项不存在");

            user.Cart.CartItems.Remove(cartItem);

            _userRespository.Update(user);
            await _cartRespository.SaveAsync();

            return InfoResult.Success();
        }

        /// <summary>
        /// 清空用户购物车
        /// </summary>
        /// <returns></returns>
        public async Task<InfoResult> ClearUserCartAsync()
        {
            var userQuary = _userRespository.AsQueryable();
            var quary = userQuary.Include(u => u.Cart)
                          .ThenInclude(c => c.CartItems)
                          .ThenInclude(ci => ci.Book)
                          .Where(u => u.UserName == _userContext.UserName);

            var user = await _userRespository.GetSingleByQueryAsync(quary);
            // 用户不存在
            if (user == null)
                return InfoResult.Fail("用户不存在");

            var cart = user.Cart;
            if (cart == null || cart.CartItems == null)
                return InfoResult.Fail("购物车为空");

            cart.CartItems.Clear();

            _userRespository.Update(user);
            await _cartRespository.SaveAsync();

            return InfoResult.Success();
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
    }
}
