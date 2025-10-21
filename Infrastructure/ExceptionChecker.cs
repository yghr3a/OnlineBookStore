using OnlineBookStore.Models.Data;

namespace OnlineBookStore.Infrastructure
{
    /// <summary>
    /// 带有作用域的异常检查器
    /// </summary>
    public static class ExceptionChecker
    {
        public static async Task<InfoResult> Scope(Func<ScopeContext, Task> action)
        {
            var context = new ScopeContext();
            await action(context);

            if (context.HasError)
                return InfoResult.Fail(context.ErrorMsg ?? "未知错误");

            return InfoResult.Success();
        }

        // 支持返回带数据的版本
        public static async Task<DataResult<T>> Scope<T>(Func<ScopeContext, Task<T?>> action)
        {
            var context = new ScopeContext();
            var data = await action(context);

            if (context.HasError)
                return DataResult<T>.Fail(context.ErrorMsg ?? "未知错误");

            return DataResult<T>.Success(data!);
        }
    }
}
