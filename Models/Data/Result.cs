using System.Reflection.Metadata.Ecma335;

namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 业务结果泛型数据类, 不想定义太多具体类型了就用这个类型
    /// </summary>
    public class Result<T>
    {
        public bool IsSuccess {  get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
        public T? Data { get; set; }

        // 一种更加便捷的创建结果对象的方式
        public static Result<T> Success(T Data) => new Result<T> { Data = Data };
        public static Result<T> Fail(string Error) => new Result<T> { ErrorMsg = Error };
    }
}
