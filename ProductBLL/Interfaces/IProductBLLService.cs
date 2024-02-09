using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductBLL.Interfaces
{
    public interface IProductBLLService
    {
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        Product AddProduct(Product product);
        Product RemoveProduct(int id);
        void DeductQuantity(int productId, int quantity);
        Task<ProductDetail> GetProductDetailById(int id);
    }
}
