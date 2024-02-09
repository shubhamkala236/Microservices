using CartDAL.Interfaces;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartDAL
{
    public class CartRepository : ICartRepository
    {
        //inmemory store
        private List<CartItem> cartItemsList = new List<CartItem>();

        public CartItem addToMyCart(CartItem cartItem)
        {
            var alreadyAdded = cartItemsList.FirstOrDefault(item => item.UserId == cartItem.UserId && item.ProductId == cartItem.ProductId);

            if (alreadyAdded != null)
            {
                return null;
            }

            cartItemsList.Add(cartItem);
            return cartItem;
        }

        public List<CartItem> getMyCartItems(int userId)
        {
            var myItems = cartItemsList.Where(item => item.UserId == userId).ToList();

            return myItems;
        }

        public void RemoveFromAllCarts(int productId)
        {
           cartItemsList.RemoveAll(item => item.ProductId == productId);
        }

        public CartItem RemovefromMyCart(int userId, int productId)
        {
            var removedItem = cartItemsList.FirstOrDefault(item => item.UserId == userId && item.ProductId == productId);
            if (removedItem != null)
            {
                cartItemsList.Remove(removedItem);
            }

            return removedItem;
        }
    }
}
