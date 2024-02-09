using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartDAL.Interfaces
{
    public interface ICartRepository
    {
        List<CartItem> getMyCartItems(int userId);
        CartItem addToMyCart(CartItem cartItem);
        CartItem RemovefromMyCart(int userId, int productId);
        void RemoveFromAllCarts(int productId); //only called on deleteProductEvent

    }
}
