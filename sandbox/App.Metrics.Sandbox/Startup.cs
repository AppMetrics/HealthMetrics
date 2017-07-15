// <copyright file="Startup.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Metrics.Sandbox
{
    public class Startup
    {
        private const bool HaveAppRunSampleRequests = true;

        public Startup(IConfiguration configuration) { Configuration = configuration; }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            app.UseTestStuff(lifetime, HaveAppRunSampleRequests);

            app.UseMetrics();
            app.UseHealthChecks();

            app.UseMvc();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.AddMetricsResourceFilter());

            services.AddMetrics().
                     AddMetricsMiddleware(
                         Configuration.GetSection("AspNetMetrics"),
                         optionsBuilder =>
                         {
                             optionsBuilder.AddAsciiFormatters();
                         });

            services.
                AddHealthChecks().
                AddHealthCheckMetrics().
                AddChecks((registry, metrics) =>
                {
                    registry.AddOveralWebRequestsApdexCheck(metrics);
                    registry.AddMetricCheck(
                        name: "Database Call Duration",
                        metrics: metrics,
                        options: SandboxMetricsRegistry.DatabaseTimer,
                        passing: value => { return (message: $"OK. 98th Percentile < 100ms ({value.Histogram.Percentile98}{SandboxMetricsRegistry.DatabaseTimer.DurationUnit.Unit()})", result: value.Histogram.Percentile98 < 100); },
                        warning: value => { return (message: $"WARNING. 98th Percentile > 100ms ({value.Histogram.Percentile98}{SandboxMetricsRegistry.DatabaseTimer.DurationUnit.Unit()})", result: value.Histogram.Percentile98 < 200); },
                        failing: value => { return (message: $"FAILED. 98th Percentile > 200ms ({value.Histogram.Percentile98}{SandboxMetricsRegistry.DatabaseTimer.DurationUnit.Unit()})", result: value.Histogram.Percentile98 > 200); });
                }).
                AddHealthCheckMiddleware(optionsBuilder => optionsBuilder.AddAsciiFormatters());
        }
    }
}