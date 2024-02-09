using Common;
using InventoryBLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryBLLService inventoryBLLService;
        public InventoryController(IInventoryBLLService inventoryBLLService)
        {
            this.inventoryBLLService = inventoryBLLService;
        }

        [HttpGet("allInventoryProducts")]
        public ActionResult<IEnumerable<Product>> GetAllInventoryProducts()
        {
            try
            {
                var products = inventoryBLLService.GetAllInventoryProducts();
                return Ok(products);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("addInventoryProduct")]
        public ActionResult<Product> AddInventoryProduct(Product product)
        {
            try
            {
                var addedProduct = inventoryBLLService.AddInventoryProduct(product);
                return addedProduct;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getInventoryProdcutById/{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            try
            {
                var product = inventoryBLLService.GetInventoryProductById(id);

                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
