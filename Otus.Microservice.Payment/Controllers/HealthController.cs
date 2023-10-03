using Microsoft.AspNetCore.Mvc;
using Otus.Microservice.TransportLibrary.Models;

namespace Otus.Microservice.Payment.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public HealthModel GetHealth()
    {
        return new HealthModel
        {
            Status = "OK"
        };
    }
}