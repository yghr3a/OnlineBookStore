namespace OnlineBookStore.Models.ViewModels
{
    /// <summary>
    /// 购物车视图模型
    /// </summary>
    public class CartViewModel
    {
        public required int UserNumber {  get; set; }

        public required List<CartItemViewModel> CartItemViewModels { get; set; }
    }
}
