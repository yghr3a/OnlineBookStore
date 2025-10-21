using OnlineBookStore.Models.Data;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 例子,外检查器, 用于检测DataResult和InfoResult的执行结果, 如果失败则抛出异常
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExceptionChecker<T>
    {
        /// 检测事务执行结果, 如果失败则抛出异常
        public static async Task<T> Check(Task<DataResult<T>> job)
        {
            var res = await job;
            if (res.IsSuccess == false)
                throw new Exception(res.ErrorMsg);      // 在这里抛出业务异常类
            return res.Data!;
        }

        public static async Task Check(Task<InfoResult> job)
        {
            var res = await job;
            if (res.IsSuccess == false)
                throw new Exception(res.ErrorMsg);      // 在这里抛出用业务异常
        }
    }
}
