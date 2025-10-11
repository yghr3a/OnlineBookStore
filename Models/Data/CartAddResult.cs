namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 购物车添加操作结果
    /// </summary>
    public class CartAddResult
    {
        // 是否添加成功
        public bool IsSuccess { get; set; }
        // 错误消息
        public string ErrorMsg { get; set; } = string.Empty;
    }
}
