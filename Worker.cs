using Quartz;
using Quartz.Impl;

namespace TestWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IScheduler _scheduler;
        private StdSchedulerFactory _stdschedulerFactory;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ScheduleJobs(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
            await _scheduler.Shutdown();
        }

        private async Task ScheduleJobs(CancellationToken stoppingToken)
        {
            _stdschedulerFactory = new StdSchedulerFactory();
            _scheduler = await _stdschedulerFactory.GetScheduler();
            await _scheduler.Start();

            IJobDetail sample=JobBuilder.Create<SampleJob>()
                .WithIdentity("job1","group")
                .Build();
            sample.JobDataMap.Add("logger", _logger);

            ITrigger trigger=TriggerBuilder.Create()
                .WithIdentity("losectrigger","group")
                .StartNow()
                .WithSimpleSchedule(config =>  config.WithIntervalInSeconds(3).WithMisfireHandlingInstructionIgnoreMisfires().RepeatForever())
                .Build();
            await _scheduler.ScheduleJob(sample,trigger, stoppingToken);

        }
    }
}
