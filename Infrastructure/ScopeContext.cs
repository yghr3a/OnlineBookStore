using OnlineBookStore.Models.Data;

namespace OnlineBookStore.Infrastructure
{
    /// <summary>
    /// 异常检测其作用域上下文
    /// </summary>
    public class ScopeContext
    {
        private bool _hasError;
        private string? _errorMsg;

        public bool HasError => _hasError;
        public string? ErrorMsg => _errorMsg;

        /// <summary>
        /// 统一执行 DataResult 方法并自动错误传播
        /// </summary>
        public async Task<T?> Get<T>(Task<DataResult<T>> func)
        {
            if (_hasError) return default; // 如果前面已经出错，就直接返回空

            var result = await func;
            if (!result.IsSuccess)
            {
                _hasError = true;
                _errorMsg = result.ErrorMsg;
                return default;
            }

            return result.Data;
        }
    }

}
