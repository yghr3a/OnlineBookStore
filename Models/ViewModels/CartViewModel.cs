namespace OnlineBookStore.Models.ViewModels
{
    /// <summary>
    /// 购物车视图模型
    /// [2025/10/16] ?为啥视图模型搞require? 删了删了
    /// </summary>
    public class CartViewModel
    {
        public int UserNumber {  get; set; }

        public List<CartItemViewModel> CartItemViewModels { get; set; } = new();
    }
}
