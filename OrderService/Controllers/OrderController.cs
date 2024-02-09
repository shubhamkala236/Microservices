using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderBLL.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderBLLService orderBLLService;
        public OrderController(IOrderBLLService orderBLLService)
        {
            this.orderBLLService = orderBLLService;
        }

        [Authorize]
        [HttpGet("myOrders")]
        public ActionResult GetMyOrders()
        {
            try
            {
                //get current User from token
                var authToken = HttpContext.Request.Headers["Authorization"];
                var userId = GetCurrentUser();
                if (userId == 0)
                {
                    return StatusCode(500, "Not Valid user please login");
                }
                var result = orderBLLService.getMyOrders(userId);
                if (result == null)
                {
                    return StatusCode(500, "Unable to get Orders");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred:{e.Message}");
            }
        }

        [Authorize]
        [HttpPost("createMyOrder/{productId}")]
        public async Task<ActionResult> CreateMyOrder(int productId)
        {
            try
            {
                var authToken = HttpContext.Request.Headers["Authorization"];
                var userId = GetCurrentUser();
                if (userId == 0)
                {
                    return StatusCode(500, "Not Valid user please login");
                }

                var result = await orderBLLService.createMyOrder(userId, productId,authToken);
                if (result == null)
                {
                    return StatusCode(500, "Unable to create order");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred:{e.Message}");
            }

        }



        //get current user
        private int GetCurrentUser()
        {
            //get userId from token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        //private int GetCurrentUser(string authToken)
        //{
        //    try
        //    {
        //        if (authToken.StartsWith("Bearer "))
        //        {
        //            authToken = authToken.Substring("Bearer ".Length);
        //        }

        //        var handler = new JwtSecurityTokenHandler();
        //        var jsonToken = handler.ReadToken(authToken) as JwtSecurityToken;

        //        if (jsonToken == null)
        //        {
        //            // Log or handle the case where the token is not a JWT
        //            return 0;
        //        }

        //        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        //        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        //        {
        //            return userId;
        //        }

        //        return 0;
        //    }
        //    catch (SecurityTokenException ex)
        //    {
        //        // Log or handle the exception
        //        Console.WriteLine($"Error parsing token: {ex.Message}");
        //        return 0;
        //    }
        //}
    }
}
