using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Buffers.Text;
using System.Security.Principal;
using System.Text.Json;
using System.Text;
using Services.ViewModels;
using Microsoft.IdentityModel.JsonWebTokens;
using MiniStore.Service.Utilities;
using System.IdentityModel.Tokens.Jwt;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthControllers : ControllerBase
    {
        private readonly IUserServices _userServices;

        public AuthControllers(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] string authToken)
        {
            try 
            { 
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(authToken);

                var payload = ((JwtSecurityToken)jsonToken).Payload.SerializeToJson();
                    
                var userLogin = JsonSerializer.Deserialize<UserLogin>(payload);
    
                LoginResponse loginResponse = null;
                UserInfo userInfo = await _userServices.FindUserByEmail(userLogin.Email);
                    
                if (userInfo != null)
                {
                    loginResponse = new LoginResponse
                    {
                        UserInfo = userInfo
                    };
                }
                else
                {
                    UserSignUp userSignUp = new UserSignUp
                    {
                        Email = userLogin.Email,
                        FullName = userLogin.FullName
                    };
                        
                    userInfo = await _userServices.CreateNewUser(userSignUp);
                        
                    if (userInfo != null)
                    {
                        loginResponse = new LoginResponse
                        {
                            UserInfo = userInfo
                        };
                    }
                }

                if (loginResponse == null || (loginResponse != null && loginResponse.UserInfo.Status != 1))
                {
                    return Unauthorized(new
                    {
                        Message = "Access denied, you are deactivated"
                    });
                }

                var accessToken = loginResponse.UserInfo.GenerateToken(AuthJWT.ACCESS_TOKEN_EXPIRED);
                var refreshToken = loginResponse.UserInfo.GenerateToken(AuthJWT.REFRESH_TOKEN_EXPIRED);
                    
                loginResponse.Token = accessToken;
                loginResponse.RefreshToken = refreshToken;

                return Ok(loginResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return Unauthorized (new 
                { 
                    Message = "Access denied, you are deactivated" 
                });
            }
        }
    }
}
