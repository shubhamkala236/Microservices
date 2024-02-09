using Common;
using InventoryBLL.Interfaces;
using InventoryDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryBLL
{
    public class InventoryBLLService : IInventoryBLLService
    {
        private readonly IInventoryRepository inventoryRepository;
        public InventoryBLLService(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository;
        }


        public Product AddInventoryProduct(Product product)
        {
            return inventoryRepository.AddInventoryProduct(product);
        }

        public List<Product> GetAllInventoryProducts()
        {
            return inventoryRepository.GetAllInventoryProducts();
        }

        public Product GetInventoryProductById(int id)
        {
            return inventoryRepository.GetInventoryProductById(id);
        }

        public Product RemoveProduct(int id)
        {
            throw new NotImplementedException();
        }
    }
}
