using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using axiosTest.Models;
using axiosTest.Dtos;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace axiosTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly BlogDbContext _blogDbContext;
        private readonly IConfiguration _configuration;

        public LoginController(BlogDbContext blogDbContext, IConfiguration configuration)
        {
            _blogDbContext = blogDbContext;
            _configuration = configuration;
        }

        [HttpPost("jwtLogin")]
        public IActionResult jwtLogin([FromBody] LoginPost value) {
            var user = (
                from a in _blogDbContext.users 
                where a.username == value.username && a.email == value.email 
                select a
                ).SingleOrDefault();
            
            // var user = _blogDbContext.users.SingleOrDefault(u => u.username == value.username);
            
            if(user == null)
                return Unauthorized("Login failed...");
            
            // new Claim(JwtRegisteredClaimNames.NameId, user.EmployeeId.ToString())
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim("FullName", user.username)
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
            return Ok(token);
        }
        
        // GET: api/<LoginController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LoginController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
