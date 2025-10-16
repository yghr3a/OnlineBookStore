using OnlineBookStore.Respository;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 订单邻域服务, 只负责纯粹的单独业务
    /// </summary>
    public class OrderDomainService
    {
        private UnitOfWork _unitOfWork;

        public OrderDomainService(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        
    }
}
