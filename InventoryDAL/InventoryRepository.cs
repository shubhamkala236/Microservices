using Common;
using InventoryDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryDAL
{
    public class InventoryRepository : IInventoryRepository
    {
        //inmemory store
        private List<Product> productList = new List<Product>();

        //get all
        public List<Product> GetAllInventoryProducts()
        {
            return productList;
        }

        //add product
        public Product AddInventoryProduct(Product product)
        {
            product.Id = productList.Count + 1;
            productList.Add(product);
            return product;
        }

        //get single product by id
        public Product GetInventoryProductById(int id)
        {
            var product = productList.FirstOrDefault(p => p.Id == id);

            if(product == null)
            {
                return null;
            }

            return product;
        }

        //remove product
        public Product RemoveProduct(int id)
        {
            throw new NotImplementedException();
        }
    }
}
