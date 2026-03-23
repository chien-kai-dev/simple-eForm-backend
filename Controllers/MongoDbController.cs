using Microsoft.AspNetCore.Mvc;
using axiosTest.Services;

namespace axiosTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MongoDbController : ControllerBase
    {
        private readonly MongoDbService _service;

        public MongoDbController(MongoDbService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}
