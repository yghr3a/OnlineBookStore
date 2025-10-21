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
        private CartFactory _cartFactory;
        private Respository<Cart> _cartRespository;

        // 在犹豫是否有必要定义一个User属性和Cart属性,直接通过UserContext获取用户Id, 然后通过AppDbContext获取用户和购物车对象似乎也挺方便的
        public CartApplication(UserContext userContext,
                           CartDomainService cartDomainService,
                           UserDomainService userDomainService,
                           BookDomainService bookDomainService,
                           CartFactory cartFactory,
                           Respository<Cart> cartRespository)
        {
            _userContext = userContext;
            _cartDomainService = cartDomainService;
            _userDomainService = userDomainService;
            _bookDomainService = bookDomainService;
            _cartFactory = cartFactory;
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

            var cartRes = await _cartDomainService.GetCartByUserIdAsync(user!.Id);
            if (cartRes.IsSuccess == false)
                return InfoResult.Fail(cartRes.ErrorMsg);
            var cart = cartRes.Data;

            // 调用领域服务的方法将书籍添加到购物车
            var addRes = await _cartDomainService.AddBookToCartAsync(book!, cart!);

            return InfoResult.Success();
        }

        /// <summary>
        /// 获取用户的购物车
        /// [2025/10/20] 业务减重重构
        /// </summary>
        public async Task<DataResult<CartViewModel>> GetUserCartAsync(int pageIndex = 1, int pageSize = 30)
        {
            // 依赖_userDomainService获取当前用户实体模型
            var userRes = await _userDomainService.GetCurrentUserEntityModelAsync();
            if (userRes.IsSuccess == false)
                return DataResult<CartViewModel>.Fail(userRes.ErrorMsg);
            var user = userRes.Data!;

            // 依赖_cartDomainService获取当前用户的cart对象
            var cartRes = await _cartDomainService.GetCartByUserIdAsync(user!.Id);
            if (cartRes.IsSuccess == false)
                return DataResult<CartViewModel>.Fail(cartRes.ErrorMsg);
            var cart = cartRes.Data!;

            // 注意!该处的cart对象的CartItems属性已经包含了所有的购物车项, 但CartItem中的Book属性并没有包含
            // 所以需要依赖_bookDomainService来批量获取这些书籍对象
            var bookIds = cart.CartItems.Select(ci => ci.BookId).ToList();
            var booksRes = await _bookDomainService.GetBookByIdAsync(bookIds);
            if (booksRes.IsSuccess == false)
                return DataResult<CartViewModel>.Fail(booksRes.ErrorMsg);
            var books = booksRes.Data!;

            // 构建cart视图模型
            var cartVMRes = _cartFactory.CreateCartViewModel(cart, books, user);
            if (cartVMRes.IsSuccess == false)
                return DataResult<CartViewModel>.Fail(cartVMRes.ErrorMsg);
            var cartVM = cartVMRes.Data!;

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
            var cartRes = await _cartDomainService.GetCartByUserIdAsync(user!.Id);
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
            var cartRes = await _cartDomainService.GetCartByUserIdAsync(user!.Id);
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
