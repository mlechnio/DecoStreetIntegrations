using DecoStreetIntegracja.Integrations;
using Quartz;
using System;

namespace DecoStreetIntegracja.Jobs
{
    public class MainIntegrationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            RunIntegrations();
        }

        public void RunIntegrations()
        {
            RunTest();
            //RunProd();
        }

        private void RunTest()
        {
            //try
            //{
            //    new D2_Kwadrat_IntegratorShoper();
            //}
            //catch (Exception ex)
            //{

            //}

            //try
            //{
            //    new Moosee_IntegrationShoper();
            //}
            //catch (Exception ex)
            //{

            //}

            try
            {
                new Kingshome_IntegrationShoper();
            }
            catch (Exception ex)
            {

            }
        }

        private void RunProd()
        {
            try
            {
                new D2_Kwadrat_Integration();
            }
            catch (Exception)
            {

            }

            try
            {
                new Malodesign_Integration();
            }
            catch (Exception)
            {

            }

            try
            {
                new Moosee_Integration();
            }
            catch (Exception)
            {

            }

            try
            {
                new Kingshome_Integration();
            }
            catch (Exception)
            {

            }

            //try
            //{
            //    new CustomFormGoogle_Integration();
            //}
            //catch (Exception)
            //{

            //}

            //try
            //{
            //    new Victoria_Integration();
            //}
            //catch (Exception ex)
            //{

            //}
        }
    }
}
