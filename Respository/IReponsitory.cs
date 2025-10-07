using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Respository
{
    /// <summary>
    /// 泛型仓储接口, 定义了基本的CRUD操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReponsitory<T> where T : IEntityModel
    {
        //public void Add(T Entity);
        //public void Update(T Entity);
        //public void Delete(T Entity);
        //public T Get();
    }
}
