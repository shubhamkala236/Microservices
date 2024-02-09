using Common;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductBLL.Interfaces;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductBLLService _productBLLService;
        private readonly IPublishEndpoint publishEndpoint;

        public ProductController(IProductBLLService _productBLLService, IPublishEndpoint publishEndpoint)
        {
            this._productBLLService = _productBLLService;
            this.publishEndpoint = publishEndpoint;
        }

        [AllowAnonymous]
        [HttpGet("allProducts")]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                var products = _productBLLService.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy ="AdminOnly")]
        [HttpPost("addProduct")]
        public ActionResult<Product> AddProduct(Product product)
        {
            try
            {
                var addedProduct = _productBLLService.AddProduct(product);
                
                return addedProduct;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpGet("getProductById/{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            try
            {
                var product = _productBLLService.GetProductById(id);

                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpGet("getProductDetailsById/{id}")]
        public async Task<ActionResult<ProductDetail>> GetProductDetailsById(int id)
        {
            try
            {
                var productDetails = await _productBLLService.GetProductDetailById(id);

                if (productDetails == null)
                    return NotFound();

                return Ok(productDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Policy ="AdminOnly")]
        [HttpDelete("removeProduct/{id}")]
        public async Task<ActionResult<Product>> RemoveProduct(int id)
        {
            try
            {
                var deletedProduct =  _productBLLService.RemoveProduct(id);
                if(deletedProduct == null)
                {
                    return NotFound();
                }

                //deleted success so send (publish) event to delete details
                await publishEndpoint.Publish<Product>(deletedProduct);


                return Ok(deletedProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
