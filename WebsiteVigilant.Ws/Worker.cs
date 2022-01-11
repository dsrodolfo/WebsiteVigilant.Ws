namespace WebsiteVigilant.Ws
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly List<string> _urls = new();  

        public Worker(ILogger<Worker> logger, 
                      IHttpClientFactory httpClientFactory,
                      IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _urls = configuration.GetSection("Urls").Get<string[]>().ToList();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await MonitorUrls();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Oops! An error occurred while processing urls.");
                }
                finally
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private async Task MonitorUrls()
        {
            List<Task> tasks = new();

            foreach (var url in _urls)
            {
                tasks.Add(MonitorURL(url));
            }

            await Task.WhenAll(tasks);
        }

        private async Task MonitorURL(string url)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("{url} is online.", url);
                }
                else
                {
                    _logger.LogWarning("{url} is offline.", url);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request to {url}.", url);
            }
        }
    }
}