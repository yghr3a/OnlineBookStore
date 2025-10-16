using System.Reflection.Metadata.Ecma335;

namespace OnlineBookStore.Models.Data
{
    /// <summary>
    /// 业务结果泛型数据类, 不想定义太多具体类型了就用这个类型
    /// </summary>
    public class ServiceResult<T>
    {
        public bool IsSuccess {  get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
        public T? Data { get; set; }

        // 一种更加便捷的创建结果对象的方式
        public static ServiceResult<T> Success(T Data) => new ServiceResult<T> { Data = Data };
        public static ServiceResult<T> Fail(string Error) => new ServiceResult<T> { ErrorMsg = Error };
    }
}
