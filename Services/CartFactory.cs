using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 购物车工厂类, 专门负责创建没有问题的Cart对象和CartItem对象, 也负责视图模型的转换
    /// </summary>
    public class CartFactory
    {
        /// <summary>
        /// 创建购物车项对象
        /// [2025/10/20] 先使用最简单的方式创建购物车项对象, 默认数量为1. 返回值类型使用DataResult包装, 方便后续添加验证逻辑
        /// </summary>
        /// <param name="book"></param>
        /// <param name="cart"></param>
        /// <returns></returns>
        public DataResult<CartItem> CreatCartItem(Book book, Cart cart)
        {
            var cartItem = new CartItem
            {
                CartId = cart.UserId,      // CardId就是UserId
                Cart = cart,
                BookId = book.Id,
                Book = book,
                Count = 1,                 // 数量先设为一, 后续可以扩展为传入参数
                CreatedDate = DateTime.Now // 设置添加时间, 这里先简单处理
            };

            return DataResult<CartItem>.Success(cartItem);
        }

        /// <summary>
        /// 创建购物车对象
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataResult<Cart> CreateCartForUser(int userId)
        {
            var cart = new Cart
            {
                UserId = userId,
            };

            return DataResult<Cart>.Success(cart);
        }

        /// <summary>
        /// 创建购物车视图模型列表
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public DataResult<List<CartItemViewModel>> CreateCartItemViewModel(Cart cart)
        {
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

            return DataResult<List<CartItemViewModel>>.Success(cartItemVMs ?? new List<CartItemViewModel>());
        }

        /// <summary>
        /// 创建购物车视图模型
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public DataResult<CartViewModel> CreateCartViewModel(Cart cart, User user)
        {
            var cartItemVMResult = CreateCartItemViewModel(cart);
            if (cartItemVMResult.IsSuccess == false)
                return DataResult<CartViewModel>.Fail(cartItemVMResult.ErrorMsg);
            var cartItemVMs = cartItemVMResult.Data;

            // 构建CartViewModel
            var cartVM = new CartViewModel()
            {
                UserNumber = user.Number,
                CartItemViewModels = cartItemVMs ?? new List<CartItemViewModel>()
            };

            return DataResult<CartViewModel>.Success(cartVM);
        }
    }
}
