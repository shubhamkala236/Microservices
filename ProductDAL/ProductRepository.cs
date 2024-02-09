using Common;
using ProductDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDAL
{
    public class ProductRepository : IProductRepository
    {
        //inmemory store
        private List<Product> productList = new List<Product>();


        public Product AddProduct(Product product)
        {
            product.Id = productList.Count + 1;
            if(product.Quantity <= 0)
            {
                product.Quantity = 1;
            }
            productList.Add(product);
            return product;
        }

        //only called by event
        public void DeductQuantity(int id, int quantity)
        {
            var product = productList.FirstOrDefault(x => x.Id == id);

            if(product != null)
            {
                var oldQuantity = product.Quantity;
                var newQuantity = oldQuantity - quantity;

                var newProduct = new Product()
                {
                    Id = product.Id,
                    Quantity = newQuantity,
                    Name = product.Name,
                };

                productList.Remove(product);
                productList.Add(newProduct); //updated product added
            }
        }

        //get all products
        public List<Product> GetAllProducts()
        {
            return productList;
        }

        //get product by id
        public Product GetProductById(int id)
        {
            var product = productList.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return null;
            }

            return product;
        }

        //remove product -- Event fires
        public Product RemoveProduct(int id)
        {
            var existingProduct = productList.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return null;
            }

            productList.Remove(existingProduct);
            return existingProduct;
        }
    }
}
