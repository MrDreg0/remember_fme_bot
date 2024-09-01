using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace RememberFMeBot.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class VersionController : ControllerBase
{
  [HttpGet]
  public IActionResult GetVersion()
  {
    var assembly = Assembly.GetExecutingAssembly();
    var version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()
      ?.Version
      ?? "Version not found";

    return Ok(version);
  }
}