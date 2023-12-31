﻿using Microsoft.AspNetCore.Mvc;
using Otus.Microservice.Order.Models;
using Otus.Microservice.TransportLibrary.Models;

namespace Otus.Microservice.Order.Controllers;

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