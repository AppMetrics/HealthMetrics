// <copyright file="SlaTestController.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace App.Metrics.Sandbox.Controllers
{
    [Route("api/[controller]")]
    public class SlaTestController : Controller
    {
        private static readonly Random Rnd = new Random();
        private readonly IMetrics _metrics;

        public SlaTestController(IMetrics metrics) { _metrics = metrics; }

        [HttpGet]
        [Route("timer")]
        public async Task<IActionResult> GetTimer()
        {
            using (_metrics.Measure.Timer.Time(SandboxMetricsRegistry.DatabaseTimer))
            {
                await Task.Delay(Rnd.Next(350), HttpContext.RequestAborted);
            }

            return Ok();
        }
    }
}