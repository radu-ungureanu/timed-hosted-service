namespace TimedHostedService
{
    public class SampleTimedService : TimedHostedService
    {
        public override int ExecutionDelaySeconds => 5;

        public SampleTimedService(ILogger<TimedHostedService> logger) : base(logger)
        {
        }

        protected override async Task RunJobAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Started running long operation at {DateTime.Now}");
            await Task.Delay(1000);
            Console.WriteLine($"Finished running long operation at {DateTime.Now}");
        }
    }
}
