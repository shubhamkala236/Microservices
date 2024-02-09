using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryDAL.Interfaces
{
    public interface IInventoryRepository
    {
        List<Product> GetAllInventoryProducts();
        Product GetInventoryProductById(int id);
        Product AddInventoryProduct(Product product);
        Product RemoveProduct(int id);
    }
}
