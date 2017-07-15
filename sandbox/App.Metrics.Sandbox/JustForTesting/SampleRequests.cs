// <copyright file="SampleRequests.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Core.Scheduling;

namespace App.Metrics.Sandbox.JustForTesting
{
    public static class SampleRequests
    {
        private const int SlaEndpointsInterval = 2;
        private static readonly Uri ApiBaseAddress = new Uri("http://localhost:1111/");

        public static void Run(CancellationToken token)
        {
            var scheduler = new DefaultTaskScheduler();
            var httpClient = new HttpClient
            {
                BaseAddress = ApiBaseAddress
            };

            Task.Run(
                () => scheduler.Interval(
                    TimeSpan.FromSeconds(SlaEndpointsInterval),
                    TaskCreationOptions.None,
                    async () =>
                    {
                        await httpClient.GetAsync("api/slatest/timer", token);
                    },
                    token),
                token);
        }
    }
}
