using Microsoft.AspNetCore.Mvc;
using axiosTest.Repositories;
using axiosTest.Models;
using axiosTest.Dtos;

namespace axiosTest.Controllers
{
	[ApiController]
	[Route("form")]
	public class FormInfoController : ControllerBase
	{
    	private readonly FormInfoRepository _formInfoRepo;
	
    	public FormInfoController(FormInfoRepository formInfoRepo)
    	{
        	// FormInfoRepository formInfoRepo
        	_formInfoRepo = formInfoRepo;
    	}
	
    	[HttpGet("all")]
    	public async Task<IActionResult> GetAll()
    	{
        	var list = await _formInfoRepo.GetAllAsync();
        	return Ok(list);
        	// return Ok(new { message = "test" });
    	}
    	
    	[HttpPost("create")]
    	public async Task<IActionResult> InsertFormInfo([FromBody] SystemPermissionApplyPost value)	{
    		// Console.WriteLine($"{value}");
    		
    		var formInfoParameters = new FormInfo {
    			FormNum = "1", 
    			CreatedDate = DateTime.Parse("2025-10-11"), 
    			CreatedTime = TimeSpan.Parse("11:20:00"), 
    			Creator = 123
    		};
    		
    		await _formInfoRepo.InsertAsync(formInfoParameters);
    		
    		// return Ok(new { token });
            // return Ok(token);
            return Ok(value);
    	}
	}
}
