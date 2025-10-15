namespace OnlineBookStore.Respository
{
    /// <summary>
    /// 工作单元模式的实现, 用于统一管理事务
    /// </summary>
    public class UnitOfWork
    {
        private readonly AppDbContext _db;
        public UnitOfWork(AppDbContext db) => _db = db;
        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

        public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken ct = default)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                await operation();               // <-- 这里你的业务逻辑在事务内执行
                await _db.SaveChangesAsync(ct);  // <-- 统一保存
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);     // 失败直接回滚
                throw;
            }
        }
    }
}
