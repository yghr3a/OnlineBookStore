using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineBookStore.Models.Data;

namespace OnlineBookStore.Infrastructure
{
    /// <summary>
    /// 带有作用域的异常检查器
    /// [2025/10/22] 目前觉得作用域并非必须, 所以添加了结果检测的检测方法
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

        /// <summary>
        /// 异步, 带数据返回的检测方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="job"></param>
        /// <returns></returns>
        public static async Task<T> CheckAsync<T>(Task<DataResult<T>> job)
        {
            var res = await job;
            if (res.IsSuccess == false)
                throw new Exception(res.ErrorMsg);  // TODO: 后续这里应该改为抛出业务异常\

            // 如果没有问题就返回数据
            return res.Data!;
        }
        
        /// <summary>
        /// 异步, 不带数据返回的版本
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task CheckAsync(Task<InfoResult> job)
        {
            var res = await job;
            if (res.IsSuccess == false)
                throw new Exception(res.ErrorMsg);  // TODO: 后续这里应该改为抛出业务异常\
        }

        /// <summary>
        /// 非异步, 带数据返回的版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="job"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T Check<T>(DataResult<T> job)
        {
            var res = job;
            if (res.IsSuccess == false)
                throw new Exception(res.ErrorMsg);  // TODO: 后续这里应该改为抛出业务异常\

            // 如果没有问题就返回数据
            return res.Data!;
        }

        /// <summary>
        /// 非异步, 不带数据返回的版本
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static void Check(InfoResult job)
        {
            var res = job;
            if (res.IsSuccess == false)
                throw new Exception(res.ErrorMsg);  // TODO: 后续这里应该改为抛出业务异常\
        }
    }
}
