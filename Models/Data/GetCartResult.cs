using OnlineBookStore.Models.Entities;
using OnlineBookStore.Models.ViewModels;

namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 获取购物车结果
    /// </summary>
    public class GetCartResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
        public CartViewModel? CartViewModel { get; set; }
    }
}
