using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartBLL.Interfaces
{
    public interface ICartBLLService
    {
        List<CartItem> getMyCartItems(int userId);
        Task<CartItem> addToMyCart(int userId, int productId, int quantity);
        CartItem RemovefromMyCart(int userId, int productId);
        void RemoveFromAllCarts(int productId); //only called on deleteProductEvent
    }
}
