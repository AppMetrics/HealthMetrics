// <copyright file="MetricsAppMetricsHealthChecksBuilderExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics;
using App.Metrics.Builder;
using App.Metrics.Health.Internal;
using App.Metrics.HealthMetrics.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
    // ReSharper restore CheckNamespace
{
    public static class MetricsAppMetricsHealthChecksBuilderExtensions
    {
        public static IAppMetricsHealthChecksBuilder AddHealthCheckMetrics(this IAppMetricsHealthChecksBuilder builder)
        {
            builder.Services.Replace(ServiceDescriptor.Singleton<IProvideHealth>(
                provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<DefaultHealthProvider>>();
                    var metrics = new Lazy<IMetrics>(() => provider.GetRequiredService<IMetrics>());
                    var defaultHealthProvider = new DefaultHealthProvider(logger, provider.GetRequiredService<IHealthCheckRegistry>());

                    return new HealthWithMetricsProvider(metrics, defaultHealthProvider);
                }));

            return builder;
        }
    }
}
