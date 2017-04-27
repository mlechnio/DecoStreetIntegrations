using DecoStreetIntegracja.Integrations;
using Quartz;

namespace DecoStreetIntegracja.Jobs
{
    public class MainIntegrationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            new D2_Kwadrat_Integration();
            new Amiou_Integration();
            new Aluro_Integration();
            new CustomForm_Integration();
            new Durbas_Integration();
        }
    }
}
