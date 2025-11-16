using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Infrastructure;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Repository;
using static OnlineBookStore.Infrastructure.ExceptionChecker;

namespace OnlineBookStore.Services
{
    public class CartApplication
    {
        private UserContext _userContext;
        private CartDomainService _cartDomainService;
        private UserDomainService _userDomainService;
        private BookDomainService _bookDomainService;
        private CartFactory _cartFactory;

        public CartApplication(UserContext userContext,
                           CartDomainService cartDomainService,
                           UserDomainService userDomainService,
                           BookDomainService bookDomainService,
                           CartFactory cartFactory)
        {
            _userContext = userContext;
            _cartDomainService = cartDomainService;
            _userDomainService = userDomainService;
            _bookDomainService = bookDomainService;
            _cartFactory = cartFactory;
        }

        /// <summary>
        /// 将书籍添加到当前登录用户的购物车
        /// </summary>
        /// [2025/10/20] 业务优化, 获取用户的工作交给UserDomainService, 获取购物车的工作交给CartDomainService, 获取书籍的工作交给BookDomainService
        /// [2025/11/03] 采用ExceptionChecker, 简化代码
        /// <param name="bookNumber"></param>
        /// <returns></returns>
        public async Task<InfoResult> AddBookToUserCart(int bookNumber)
        {
            try
            {
                // 获取用户实体对象和购物车实体对象
                var user = await CheckAsync(_userDomainService.GetCurrentUserEntityModelAsync());
                var cart = await CheckAsync(_cartDomainService.GetCartByUserIdAsync(user.Id));
                var book = await CheckAsync(_bookDomainService.GetBookByNumberAsync(bookNumber));

                // 将书籍添加到购物车里
                await (_cartDomainService.AddBookToCartAsync(book!, cart!));

                return InfoResult.Success();
            }
            catch (Exception ex)
            {
                return InfoResult.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 获取用户的购物车
        /// </summary>
        /// [2025/10/20] 业务减重重构
        /// [2025/11/03] 采用ExceptionChecker简化代码
        public async Task<DataResult<CartViewModel>> GetUserCartAsync(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                // 获取用户实体对象和购物车实体对象
                var user = await CheckAsync(_userDomainService.GetCurrentUserEntityModelAsync());
                var cart = await CheckAsync(_cartDomainService.GetCartByUserIdAsync(user.Id));

                // 获取书籍列表
                var bookIds = cart.CartItems.Select(ci => ci.BookId).ToList();
                var books = await CheckAsync(_bookDomainService.GetBookByIdAsync(bookIds));

                // 构建购物车视图模型
                var cartVM = Check(_cartFactory.CreateCartViewModel(cart, books, user));

                // TODO: 后续再考虑将分页操作下放领域类处理
                // 分页调整cartVM里的CartItemViewModels
                var cartItems = cartVM.CartItemViewModels.Skip(pageIndex -1).Take(pageSize).ToList();
                cartVM.CartItemViewModels = cartItems;

                return DataResult<CartViewModel>.Success(cartVM);
            }
            catch (Exception ex)
            {
                return DataResult<CartViewModel>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 获取被搜索的用户购物车
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<DataResult<CartViewModel>> GetSearchedUserCartAsync(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                // 获取用户实体对象和购物车实体对象
                var user = await CheckAsync(_userDomainService.GetCurrentUserEntityModelAsync());
                var cart = await CheckAsync(_cartDomainService.GetCartByUserIdAsync(user.Id));

                // 获取书籍列表
                var bookIds = cart.CartItems.Select(ci => ci.BookId).ToList();
                var books = await CheckAsync(_bookDomainService.GetBookByIdAsync(bookIds));

                // 构建购物车视图模型
                var cartVM = Check(_cartFactory.CreateCartViewModel(cart, books, user));

                // TODO: 后续再考虑将关键字查询操作下放领域类处理
                // 构建关键字查询
                var query = cartVM.CartItemViewModels.AsQueryable();
                if(string.IsNullOrWhiteSpace(keyword) == false)
                {
                    query = query.Where(ci =>
                    ci.BookTitle.Contains(keyword) ||
                    ci.BookAuthor.Contains(keyword));
                }

                // TODO: 后续再考虑将分页操作下放领域类处理
                // 分页查询
                var cartItems = query.Skip(pageIndex - 1).Take(pageSize).ToList();
                cartVM.CartItemViewModels = cartItems;

                return DataResult<CartViewModel>.Success(cartVM);
            }
            catch (Exception ex)
            {
                return DataResult<CartViewModel>.Fail(ex.Message);
            }
        }


        /// <summary>
        /// 移除用户购物车中的单个项
        /// </summary>
        /// [2025/10/20] 业务优化, 获取购物车的工作交给CartDomainService
        /// [2025/10/20] 再次重构, 为了避免领域模型之间边界混乱, 所以我们发挥Application层的协调作用, 让Application层来处理跨领域模型的业务逻辑
        /// 说白了就是让Application调用UserDomainService获取user, 然后调用CartDomainService获取cart
        /// [2025/11/03] 采用ExceptionCheck简化代码
        /// <param name="bookNumber"></param>
        /// <returns></returns>
        public async Task<InfoResult> RemoveUserCartSingleItemAsync(int cartItemNumber)
        {
            try
            {
                // 获取用户实体对象和购物车实体对象
                var user = await CheckAsync(_userDomainService.GetCurrentUserEntityModelAsync());
                var cart = await CheckAsync(_cartDomainService.GetCartByUserIdAsync(user!.Id));

                // 移除操作
                await CheckAsync(_cartDomainService.RemoveCartItemByCartItemNumberAsync(cart, cartItemNumber));

                return InfoResult.Success();
            }
            catch (Exception ex)
            {
                return InfoResult.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 清空用户购物车
        /// </summary>
        /// [2025/10/20] 业务优化, 获取购物车的工作交给CartDomainService
        /// [2025/10/20] 再次重构, 为了避免领域模型之间边界混乱, 所以我们发挥Application层的协调作用, 让Application层来处理跨领域模型的业务逻辑
        /// 说白了就是让Application调用UserDomainService获取user, 然后调用CartDomainService获取cart
        /// [2025/10/22] 开始引用ExceptionCheck
        /// [2025/11/03] 依赖_cartDomainService来更新实体对象信息而不是直接依赖仓储对象
        /// <returns></returns>
        public async Task<InfoResult> ClearUserCartAsync()
        {
            try 
            {
                // 极大的简化了写法
                var user = await CheckAsync(_userDomainService.GetCurrentUserEntityModelAsync());
                var cart = await CheckAsync(_cartDomainService.GetCartByUserIdAsync(user!.Id));

                cart!.CartItems.Clear();

                await CheckAsync(_cartDomainService.UpdateAsync(cart));

                return InfoResult.Success();
            }
            catch(Exception ex) // 后续记得加上业务异常
            {
                return InfoResult.Fail(ex.Message);
            }
        }
    }
}
