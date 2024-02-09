using CartBLL.Interfaces;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartBLLService cartBLLService;
        public CartController(ICartBLLService cartBLLService)
        {
            this.cartBLLService = cartBLLService;
        }

        [Authorize]
        [HttpGet("myCart")]
        public ActionResult GetMyCart()
        {
            try
            {
                //get current User from token
                //var userId = GetCurrentUser();
                //var authToken = HttpContext.Request.Headers["Authorization"];
                var userId = GetCurrentUser();
                if (userId == 0)
                {
                    return StatusCode(500, "Not Valid user please login");
                }
                var result = cartBLLService.getMyCartItems(userId);
                if (result == null)
                {
                    return StatusCode(500, "Unable to get cart items");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred:{e.Message}");
            }
        }

        //[HttpGet("myCartbyId/{myId}")]
        //public ActionResult GetMyCartById(int myId)
        //{
        //    try
        //    {
        //        //get current User from token
        //        //var userId = GetCurrentUser();
        //        var authToken = HttpContext.Request.Headers["Authorization"];
        //        var userId = GetCurrentUser(authToken);
        //        if (userId == 0)
        //        {
        //            return StatusCode(500, "Not Valid user please login");
        //        }
        //        if(myId != userId)
        //        {
        //            return StatusCode(500, "Not Valid user please login");
        //        }
        //        //my id same as logged in userId
        //        var result = cartBLLService.getMyCartItems(userId);
        //        if (result == null)
        //        {
        //            return StatusCode(500, "Unable to get cart items");
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, $"An error occurred:{e.Message}");
        //    }
        //}

        [Authorize]
        [HttpPost("addToMyCart/{productId}/{quantity}")]
        public async Task<ActionResult> AddToMyCart(int productId, int quantity)
        {
            try
            {
                //var userId = GetCurrentUser();
                //var authToken = HttpContext.Request.Headers["Authorization"];
                var userId = GetCurrentUser();
                if (userId == 0)
                {
                    return StatusCode(500, "Not Valid user please login");
                }

                var result = await cartBLLService.addToMyCart(userId, productId, quantity);
                if (result == null)
                {
                    return StatusCode(500, "Unable to Add cart items");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred:{e.Message}");
            }

        }

        [Authorize]
        [HttpDelete("removeFromMyCart/{productId}")]
        public ActionResult RemoveFromMyCart(int productId)
        {
            try
            {
                //var userId = GetCurrentUser();
                //var authToken = HttpContext.Request.Headers["Authorization"];
                var userId = GetCurrentUser();
                if (userId == 0)
                {
                    return StatusCode(500, "Not Valid user please login");
                }

                var result = cartBLLService.RemovefromMyCart(userId,productId);
                if (result == null)
                {
                    return StatusCode(500, "Unable to remove cart item");
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
