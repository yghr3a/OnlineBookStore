namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 通用信息结果, 不带泛型数据
    /// </summary>
    public class InfoResult
    {
        public bool IsSuccess {  get; set; }
        public string ErrorMsg { get; set; } = string.Empty;

        public static InfoResult Success() => new InfoResult { IsSuccess = true };
        public static InfoResult Fail(string errorMsg) => new InfoResult { IsSuccess = false, ErrorMsg = errorMsg };
    }
}
