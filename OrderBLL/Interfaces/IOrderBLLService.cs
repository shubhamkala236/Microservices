using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBLL.Interfaces
{
    public interface IOrderBLLService
    {
        List<Order> getMyOrders(int userId);
        Task<Order> createMyOrder(int userId, int productId, string authToken);
    }
}
