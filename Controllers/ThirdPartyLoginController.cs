using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using System.Threading.Tasks;

using axiosTest.Services;
using axiosTest.Models;
// using Microsoft.AspNetCore.Identity;

namespace axiosTest.Controllers	
{
	[ApiController]
	[Route("auth")]
	public class ThirdPartyLoginController : ControllerBase    
	{
    	private readonly IConfiguration _configuration;
    	// private readonly UserManager<GoogleAuthUser> _userManager;
    	
    	private readonly IRedisCacheService _cacheService;
		private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);
    	
    	public ThirdPartyLoginController(IConfiguration configuration, IRedisCacheService cacheService) 
    	{
        	_configuration = configuration;
        	_cacheService = cacheService;
        	// _userManager = userManager;
    	}
	
    	/*
    	[HttpGet("google")]
    	public IActionResult GoogleLogin() 
    	{
        	var properties = new AuthenticationProperties {
            	RedirectUri = Url.Action("GoogleCallback")
        	}; 
        	
        	return Challenge(properties, "Google");
    	}
    	*/
	
    	[HttpGet("token/check")]
    	public async Task<IActionResult> CheckToken() 
    	{
        	foreach (var cookie in Request.Cookies)
			{
    			Console.WriteLine($"Cookie received: {cookie.Key} = {cookie.Value}");
			}
        	
        	var getToken = Request.Cookies.TryGetValue("token", out var token);
        	bool status = false;
        	Console.WriteLine($"{getToken}");
        	
        	if(getToken)
        	{
        		var cacheKey = $"Info:{token}";
        		var tokenCached = _cacheService.GetCacheValueAsync<LoginUserInfo>(cacheKey);
        		
        		if(tokenCached != null)
        		{
        			status = true;
        		}
        	}
        	
        	return Ok(new { message = status });
    	}
    	
    	[HttpPost("google/callback")]
    	public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginRequest request)
    	{
        	var token = GenerateJwtToken(request.email);
        	// return Ok(new { token });
        	
        	var cacheKey = $"Info:{token}";
        	// var cacheKey = $"user:123";
        	var data = new LoginUserInfo
    		{
        		UserName = request.name,
            	Email = request.email,
            	Provider = request.provider,
            	ProviderId = request.providerId
    		};
    		
    		await _cacheService.SetCacheValueAsync(cacheKey, data, CacheExpiration);
    		
    		// Set Cookie
    		Response.Cookies.Append("token", token, new CookieOptions
        	{
            	HttpOnly = true,
            	Secure = true, 
            	SameSite = SameSiteMode.Strict,
            	Expires = DateTimeOffset.UtcNow.AddMinutes(10), 
            	IsEssential = true
        	});
        	
        	// return Ok(new { message = "Success to login" });
        	return Ok(new { message = token });
    	}
	
    	public string GenerateJwtToken(string email)
    	{
        	var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, email)
            };
        	
        	var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var jwt = new JwtSecurityToken
            (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            // return Ok(new { token });
            // return Ok(token);
            return token;
    	}
	}
}
