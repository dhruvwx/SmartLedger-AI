 using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var versionObject = new
            {
                version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
                environments = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                deployedAt = System.IO.File.GetCreationTimeUtc(typeof(Program).Assembly.Location)
                            };
            return Ok(versionObject);
        }
    }
}
