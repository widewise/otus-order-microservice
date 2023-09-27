using Microsoft.AspNetCore.Mvc;
using Otus.Microservice.Delivery.Models;

namespace Otus.Microservice.Delivery.Controllers;

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