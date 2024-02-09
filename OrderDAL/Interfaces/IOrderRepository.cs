using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDAL.Interfaces
{
    public interface IOrderRepository
    {
        List<Order> getMyOrders(int userId);
        Order createMyOrder(Order order);
    }
}
