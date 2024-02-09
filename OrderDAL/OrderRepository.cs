using Common;
using OrderDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDAL
{
    public class OrderRepository : IOrderRepository
    {
        //inmemory store
        private List<Order> orderList = new List<Order>();

        public Order createMyOrder(Order order)
        {
            orderList.Add(order);
            return order;
        }

        public List<Order> getMyOrders(int userId)
        {
            var myOrders = orderList.Where(order => order.UserId == userId).ToList();

            return myOrders;
        }
    }
}
