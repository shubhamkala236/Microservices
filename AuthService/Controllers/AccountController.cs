using JwtAuthenticationManager;
using JwtAuthenticationManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtHandler jwtHandler;
        public AccountController(JwtHandler jwtHandler)
        {
            this.jwtHandler = jwtHandler;
        }

        [HttpPost]
        public ActionResult<AuthenticationResponse?> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            var response = jwtHandler.GenerateJwtToken(authenticationRequest);
            if(response == null)
            {
                return Unauthorized();
            }

            return response;
        }

        [HttpGet]
        public string Get()
        {
            return "Hi from auth";
        }
    }
}
