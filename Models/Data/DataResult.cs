using System.Reflection.Metadata.Ecma335;

namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 结果泛型通用数据类
    /// </summary>
    public class DataResult<T>
    {
        public bool IsSuccess {  get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
        public T? Data { get; set; }

        // 一种更加便捷的创建结果对象的方式
        public static DataResult<T> Success(T Data) => new DataResult<T> { IsSuccess = true, Data = Data };
        public static DataResult<T> Fail(string Error) => new DataResult<T> { IsSuccess = false, ErrorMsg = Error };
    }
}
