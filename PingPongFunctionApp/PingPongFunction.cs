using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PingPongFunctionApp
{
    public class PingPongFunction
    {
        private readonly HttpClient _httpClient;
        private readonly PingPongOptions _settings;
        private readonly ILogger<PingPongFunction> _logger;

        public PingPongFunction(HttpClient httpClient, IOptions<PingPongOptions> options, ILogger<PingPongFunction> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _settings = options.Value;
        }

        [FunctionName("PingPongFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("{name} player processed a request.", _settings.PlayerName);

            await Task.Delay(_settings.DelaySeconds);
            await _httpClient.PostAsync(_settings.OpponentUri, new StringContent("ping"));

            _logger.LogInformation("{name} player sent a request.");

            return new OkResult();
        }
    }
}

