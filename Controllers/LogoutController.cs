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
// using axiosTest.Models;

namespace axiosTest.Controllers	
{
	[ApiController]
	[Route("auth")]
	public class LogoutController : ControllerBase    
	{
    	private readonly IRedisCacheService _cacheService;
    	
    	public LogoutController(IRedisCacheService cacheService) 
    	{
        	_cacheService = cacheService;
    	}
	
    	[HttpGet("token/delete")]
    	public async Task<IActionResult> RemoveToken() 
    	{
        	var getToken = Request.Cookies.TryGetValue("token", out var token);
        	bool status = true;
        	
        	var cacheKey = $"Info:{token}";
        	var deleteTokenCached = _cacheService.RemoveCacheValueAsync(cacheKey);
        	Response.Cookies.Delete("token");
        	
        	return Ok(new { message = status });
    	}
	}
}
