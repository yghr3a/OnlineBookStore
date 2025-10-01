using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Repository
{
    /// <summary>
    /// 泛型仓储类, 提供基本的增删查改操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> where T : class, IEntityModel
    {
        private AppDbContext _context;
        private DbSet<T> _dbSet;

        // 依赖注入, 获取应用数据库上下文
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // 获取全部实体
        public async Task<IEnumerable<T>> GetAllAsync() { return await _dbSet.ToListAsync(); }
        // 根据Id获取实体
        public async Task<T> GetByIdAsync(int id) { return await _dbSet.FindAsync(id); }
        // 添加实体
        public async Task Add(T entity) {await _dbSet.AddAsync(entity); }
        // 删除实体
        public void  Delete(T entity) { _dbSet.Remove(entity); }
        // 更改实体
        public void Update(T entity) {_dbSet.Update(entity); }
        // 保存更改
        public async Task Save() {await _context.SaveChangesAsync(); }
    }
}
