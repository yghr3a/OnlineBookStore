using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Repository
{
    /// <summary>
    /// 泛型仓储类, 提供基本的增删查改操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> where T : class, IEntityModel
    {
        private readonly IServiceScopeFactory _serviceFactory;  // 域服务构造工厂, 在高并发的场景下构造多个DbContext实例
        private AppDbContext _context;
        private DbSet<T> _dbSet;

        // 依赖注入, 获取应用数据库上下文
        public Repository(AppDbContext context, IServiceScopeFactory serviceFactory)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _serviceFactory = serviceFactory;   
        }

        // 异步获取全部实体
        // 注意这里返回的是 List<T> 而不是 IEnumerable<T>, 是因为_dbset.ToListAsync() 返回的是 List<T>
        public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync(); 
        // 异步根据Id获取实体
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id); 
        // 异步添加实体
        public async Task AddAsync(T entity) { await _dbSet.AddAsync(entity); }
        // 删除实体
        public void Delete(T entity) { _dbSet.Remove(entity); }
        // 更改实体
        public void Update(T entity) {_dbSet.Update(entity); }
        // 保存更改
        public async Task SaveAsync() { await _context.SaveChangesAsync(); }

        /// <summary>
        /// 获取可查询的实体集合
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> AsQueryable() => _dbSet.AsQueryable();

        // 个人觉得这个方法主要代替GetAllAsync, 因为按需分页获取数据更符合实际需求, 避免一次性加载过多数据
        /// <summary>
        /// 异步获取分页实体
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<T>> GetPagedAsync(int pageIndex, int pageSize)
        {
            return await _dbSet.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// 异步获取分页实体, 支持自定义查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<T>> GetPagedAsync(IQueryable<T> query, int pageIndex, int pageSize)
        {
            return await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // [2025/10/4] 书籍编号查询,订单编号查询等操作需要使用编号进行查询, Id是主键, 不应该暴露给外部使用
        /// <summary>
        /// 异步根据自定义查询获取单个实体
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<T?> GetSingleByQueryAsync(IQueryable<T> query)
        {
            return await query.FirstOrDefaultAsync();
        }

        // [2025/10/4] 根据自定义查询获取实体列表, 主要用于搜索等功能; 来自于GetPagedAsync方法的灵感
        /// <summary>
        /// 异步根据自定义查询获取实体列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<T>> GetListByQueryAsync(IQueryable<T> query)
        {
            return await query.ToListAsync();
        }

        /// <summary>
        /// 并行添加实体, 适用于需要同时添加多个实体的场景, 可以提高性能; 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task ParallelAddAsync(IEnumerable<T> entities, int batchsize = 20, int max_concurrent = 5)
        {
            // 使用semaphoreslim来限制最大并发数, 使用using自动释放资源
            using var semaphore = new SemaphoreSlim(max_concurrent);

            var tasks = new List<Task>();
            var count = entities.Count();
            for (int i = 0; i < count; i += batchsize)
            {
                var batch = entities.Skip(i).Take(batchsize).ToList();
                tasks.Add(Task.Run(async () =>
                {
                    // 异步环境优先使用异步等待
                    await semaphore.WaitAsync();

                    try
                    {
                        using (var scope = _serviceFactory.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            await context.Set<T>().AddRangeAsync(batch);
                            await context.SaveChangesAsync();
                        }
                    }
                    finally 
                    {
                        // 最终释放
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

    }
}


