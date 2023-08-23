namespace TimedHostedService
{
    public abstract class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer? _timer;
        private Task? _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new();
        private TimeSpan _executionDelay;

        public abstract int ExecutionDelaySeconds { get; }

        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
            _executionDelay = TimeSpan.FromSeconds(ExecutionDelaySeconds);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(ExecuteTask, null, _executionDelay, Timeout.InfiniteTimeSpan);

            return Task.CompletedTask;
        }

        private void ExecuteTask(object? state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _executingTask = ExecuteTaskAsync(_stoppingCts.Token);
        }

        private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
        {
            await RunJobAsync(stoppingToken);
            _timer?.Change(_executionDelay, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// This method is called when the <see cref="IHostedService"/> starts. The implementation should return a task 
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        protected abstract Task RunJobAsync(CancellationToken stoppingToken);

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);

            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }

        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
            _timer?.Dispose();
        }
    }
}
