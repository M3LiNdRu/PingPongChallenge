using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(PingPongFunctionApp.Startup))]

namespace PingPongFunctionApp
{
    class Startup : FunctionsStartup
    {
        public static PingPongOptions Options { get; set; }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<PingPongOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                var section = configuration.GetSection("PingPongOptions");
                Options = section.Get<PingPongOptions>();
                section.Bind(settings);
            });

            builder.Services.AddHttpClient<PingPongFunction>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) 
                .AddPolicyHandler(GetRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            Random jitterer = new Random();
            var retryWithJitterPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );

            return retryWithJitterPolicy;
        }
    }
}
