﻿using AutoMapper.Internal.Mappers;
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
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System.Net.Sockets;

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
                UserInfo userInfo = await _userServices.FindUserByEmail(userLogin.email);
                    
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
                        Email = userLogin.email,
                        FullName = userLogin.name
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
                return BadRequest (new 
                { 
                    Message = "Login Failed: " + e.Message 
                });
            }
        }

        [HttpPost("refresh-token")]

        public async Task<IActionResult> refreshToken([FromBody] string tokenRequest)
        {
            if (!ValidateToken(tokenRequest))
            {
                return Unauthorized("Invalid refresh token");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadToken(tokenRequest);

            var payload = ((JwtSecurityToken)jsonToken).Payload.SerializeToJson();

            var userLogin = JsonSerializer.Deserialize<RefreshLogin>(payload);


            string email = userLogin.nameid;
            UserInfo userInfo = await _userServices.FindUserById(Guid.Parse(userLogin.nameid));
            
            if (userInfo == null)
            {
                return Unauthorized("Token claim is invalid");
            }

            var loginResponse = new LoginResponse
            {
                UserInfo = userInfo
            };

            string newToken = loginResponse.UserInfo.GenerateToken(AuthJWT.ACCESS_TOKEN_EXPIRED);
            string newRefreshToken = loginResponse.UserInfo.GenerateToken(AuthJWT.REFRESH_TOKEN_EXPIRED);

            return Ok(loginResponse);
        }

        private bool ValidateToken(String token)
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtAuth:Key"]))
                };

                // Validate the token
                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                return true;
            }
            catch (SecurityTokenSignatureKeyNotFoundException e)
            {
                Console.WriteLine("Invalid JWT signature: {0}", e.Message);
            }
            catch (SecurityTokenInvalidSignatureException e)
            {
                Console.WriteLine("Invalid JWT token: {0}", e.Message);
            }
            catch (SecurityTokenExpiredException e)
            {
                Console.WriteLine("JWT token is expired: {0}", e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("JWT claims string is empty: {0}", e.Message);
            }

            return false;
        }
    }

}
