using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductDetailBLL.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductDetailService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailController : ControllerBase
    {
        private readonly IProductDetailBLLService productDetailBLLService;
        public ProductDetailController(IProductDetailBLLService productDetailBLLService)
        {
            this.productDetailBLLService = productDetailBLLService;
        }

        [AllowAnonymous]
        [HttpGet("getAllProductDetails")]
        public ActionResult<IEnumerable<ProductDetail>> GetAllProductDetails()
        {
            try
            {
                var productDetails = productDetailBLLService.GetAllProductDetails();
                return Ok(productDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy ="AdminOnly")]
        [HttpPost("addProductDetails")]
        public async Task<ProductDetail> AddProductDetails(ProductDetail productDetail)
        {
            try
            {
                var addedDetails = await productDetailBLLService.AddProductDetails(productDetail);
                if(addedDetails == null)
                {
                    throw new Exception($"Unable to add ProductDetails");
                }
                return addedDetails;
            }
            catch(Exception ex)
            {
                throw new Exception($"Unable to add ProductDetails Error:{ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("getProductDetailsById/{id}")]
        public ActionResult<ProductDetail> GetAllProductDetailsById(int id)
        {
            try
            {
                var productDetails = productDetailBLLService.GetProductDetailsById(id);
                if (productDetails == null)
                    return NotFound();
                return Ok(productDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("removeProductDetail/{id}")]
        public async Task<ActionResult<ProductDetail>> RemoveProductDetail(int id)
        {
            try
            {
                var productDetails = await productDetailBLLService.RemoveProductDetails(id);
                if (productDetails == null)
                    return NotFound();
                return Ok(productDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private int GetCurrentUser(string authToken)
        {
            try
            {
                if (authToken.StartsWith("Bearer "))
                {
                    authToken = authToken.Substring("Bearer ".Length);
                }

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(authToken) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    // Log or handle the case where the token is not a JWT
                    return 0;
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                return 0;
            }
            catch (SecurityTokenException ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error parsing token: {ex.Message}");
                return 0;
            }
        }

    }
}
