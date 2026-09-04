namespace OnlineBookStore.Models.Exceptions
{
    /// <summary>
    /// 领域(业务)异常基类。
    /// 领域模型与领域服务在业务规则被违反时抛出该异常,
    /// 由应用层捕获并转换为面向调用方的结果对象(InfoResult/DataResult)。
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }

        public DomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
