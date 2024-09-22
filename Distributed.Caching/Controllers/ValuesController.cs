using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Distributed.Caching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly IDistributedCache _distributedCache;

        public ValuesController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet("set")]
        public async Task<IActionResult> Set(string name, string surname)
        {
            await _distributedCache.SetStringAsync("name", name, options: new()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1),
                SlidingExpiration = TimeSpan.FromSeconds(30)
            });
            await _distributedCache.SetAsync("surname",Encoding.UTF8.GetBytes(surname),options: new()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1),
                SlidingExpiration = TimeSpan.FromSeconds(30)
            });

            return Ok();
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {

            var name = await _distributedCache.GetStringAsync("name");
            var surname = await _distributedCache.GetAsync("surname");
            var surnameString = string.Empty;
            if(surname != null)
            {
                surnameString = Encoding.UTF8.GetString(surname);
            }
            return Ok(new
            {
                name,
                surnameString
            });
        }
    }
}
