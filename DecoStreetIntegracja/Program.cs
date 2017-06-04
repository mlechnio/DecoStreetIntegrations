using DecoStreetIntegracja.Integrations;
using DecoStreetIntegracja.Jobs;
using DecoStreetIntegracja.Utils;
using Quartz;
using Quartz.Impl;
using System.Configuration;

namespace DecoStreetIntegracja
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<MainIntegrationJob>().Build();

            var trigger = TriggerBuilder.Create()
                            .WithCronSchedule(StringCostants.CRON_SCHEDULE)
                            .StartNow()
                            .Build();

            scheduler.ScheduleJob(job, trigger);
//
            new MainIntegrationJob().RunIntegrations();
        }
    }
}
