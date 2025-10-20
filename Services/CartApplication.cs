using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Respository;

namespace OnlineBookStore.Services
{
    public class CartApplication
    {
        // 用户上下文信息
        private UserContext _userContext;
        private CartDomainService _cartDomainService;
        private UserDomainService _userDomainService;
        private BookDomainService _bookDomainService;
        private Respository<User> _userRespository;
        private Respository<Cart> _cartRespository;

        // 在犹豫是否有必要定义一个User属性和Cart属性,直接通过UserContext获取用户Id, 然后通过AppDbContext获取用户和购物车对象似乎也挺方便的
        public CartApplication(UserContext userContext,
                           CartDomainService cartDomainService,
                           UserDomainService userDomainService,
                           BookDomainService bookDomainService,
                           Respository<User> userRespository,
                           Respository<Cart> cartRespository)
        {
            _userContext = userContext;
            _cartDomainService = cartDomainService;
            _userDomainService = userDomainService;
            _bookDomainService = bookDomainService;

            _userRespository = userRespository;
            _cartRespository = cartRespository;
        }

        /// <summary>
        /// 将书籍添加到当前登录用户的购物车
        /// [2025/10/20] 业务优化, 获取用户的工作交给UserDomainService, 获取购物车的工作交给CartDomainService, 获取书籍的工作交给BookDomainService
        /// </summary>
        /// <param name="bookNumber"></param>
        /// <returns></returns>
        public async Task<InfoResult> AddBookToUserCart(int bookNumber)
        {
            var userRes = await _userDomainService.GetCurrentUserEntityModelAsync();
            if (userRes.IsSuccess == false)
                return InfoResult.Fail(userRes!.ErrorMsg);
            var user = userRes.Data;

            var bookRes = await _bookDomainService.GetBookByNumberAsync(bookNumber);
            if (bookRes.IsSuccess == false)
                return InfoResult.Fail(bookRes!.ErrorMsg);
            var book = bookRes.Data;

            var cartRes = await _cartDomainService.GeCartByUserIdAsync(user!.Id);
            if (cartRes.IsSuccess == false)
                return InfoResult.Fail(cartRes.ErrorMsg);
            var cart = cartRes.Data;

            // 调用领域服务的方法将书籍添加到购物车
            var addRes = await _cartDomainService.AddBookToCartAsync(book!, cart!);

            return InfoResult.Success();
        }

        /// <summary>
        /// 获取用户的购物车
        /// </summary>
        public async Task<DataResult<CartViewModel>> GetUserCartAsync(int pageIndex = 1, int pageSize = 30)
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
            if (user is null)
                return DataResult<CartViewModel>.Fail("用户不存在");

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
            return DataResult<CartViewModel>.Success(cartVM);
        }


        /// <summary>
        /// 移除用户购物车中的单个项
        /// [2025/10/20] 业务优化, 获取购物车的工作交给CartDomainService
        /// [2025/10/20] 再次重构, 为了避免领域模型之间边界混乱, 所以我们发挥Application层的协调作用, 让Application层来处理跨领域模型的业务逻辑
        /// 说白了就是让Application调用UserDomainService获取user, 然后调用CartDomainService获取cart
        /// </summary>
        /// <param name="bookNumber"></param>
        /// <returns></returns>
        public async Task<InfoResult> RemoveUserCartSingleItemAsync(int orderItemNumber)
        {
            // 获取当前用户实体模型
            var userRes = await _userDomainService.GetCurrentUserEntityModelAsync();
            if (userRes.IsSuccess == false)
                return InfoResult.Fail(userRes.ErrorMsg);
            var user = userRes.Data;

            // 依赖_cartDomainService获取当前用户的cart对象
            var cartRes = await _cartDomainService.GeCartByUserIdAsync(user!.Id);
            if (cartRes.IsSuccess == false)
                return InfoResult.Fail(cartRes.ErrorMsg);
            var cart = cartRes.Data;

            // 购物车项的Id目前是暴露出来作为Number使用的
            var cartItem = cart!.CartItems.FirstOrDefault(ci => ci.Id == orderItemNumber);
            if (cartItem is null)
                return InfoResult.Fail("目标购物车项不存在");

            cart.CartItems.Remove(cartItem);

            _cartRespository.Update(cart);
            await _cartRespository.SaveAsync();

            return InfoResult.Success();
        }

        /// <summary>
        /// 清空用户购物车
        /// [2025/10/20] 业务优化, 获取购物车的工作交给CartDomainService
        /// [2025/10/20] 再次重构, 为了避免领域模型之间边界混乱, 所以我们发挥Application层的协调作用, 让Application层来处理跨领域模型的业务逻辑
        /// 说白了就是让Application调用UserDomainService获取user, 然后调用CartDomainService获取cart
        /// </summary>
        /// <returns></returns>
        public async Task<InfoResult> ClearUserCartAsync()
        {
            // 获取当前用户实体模型
            var userRes = await _userDomainService.GetCurrentUserEntityModelAsync();
            if (userRes.IsSuccess == false)
                return InfoResult.Fail(userRes.ErrorMsg);
            var user = userRes.Data;

            // 依赖_cartDomainService获取当前用户的cart对象
            var cartRes = await _cartDomainService.GeCartByUserIdAsync(user!.Id);
            if (cartRes.IsSuccess == false)
                return InfoResult.Fail(cartRes.ErrorMsg);
            var cart = cartRes.Data;

            cart!.CartItems.Clear();

            _cartRespository.Update(cart);
            await _cartRespository.SaveAsync();

            return InfoResult.Success();
        }
    }
}
