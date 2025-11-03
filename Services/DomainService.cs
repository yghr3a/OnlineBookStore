using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;
using OnlineBookStore.Models.Data;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 领域服务泛型基类
    /// </summary>
    /// 基本上一个领域服务管理着一个聚合根, T类型对应着聚合根的类型
    /// <typeparam name="T"></typeparam>
    public abstract class DomainService<T> where T : class, IEntityModel
    {
        protected Repository<T> _repository;

        public DomainService(Repository<T> respository)
        {
            _repository = respository;
        }

        /// <summary>
        /// 更新实体所对应数据项的方法
        /// </summary>
        /// 如果应用层需要更新数据项的属性, 领域服务类需要提供方法供应用类实现数据更新, 不能让应用类直接依赖仓储类
        /// 而且为每一个领域服务类型写一个更新方法太麻烦了, 于是干脆定义一个基类, 减少工作量. 由于是为了代码复用而不是抽象, 最好使用基类来实现
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<InfoResult> UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await _repository.SaveAsync();

            return InfoResult.Success();
        }
    }
}
