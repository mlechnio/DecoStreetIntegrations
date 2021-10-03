using DecoStreetIntegracja.Integrations;
using DecoStreetIntegracja.Utils;
using Quartz;
using System;
using System.Collections.Generic;

namespace DecoStreetIntegracja.Jobs
{
    public class MainIntegrationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Logger.Clear();
            Logger.Log("Integrations Started");
            RunIntegrations();
            Logger.Log("Integrations Ended");
            new EmailSender().SendLogs();
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
            //    Logger.Log("D2_Kwadrat Started");
            //    new D2_Kwadrat_IntegratorShoper();
            //    Logger.Log("D2_Kwadrat Ended");
            //}
            //catch (Exception ex)
            //{
            // Logger.LogException(ex);
            //}

            //try
            //{
            //    Logger.Log("Moosee Started");
            //    new Moosee_IntegrationShoper();
            //    Logger.Log("Moosee Ended");
            //}
            //catch (Exception ex)
            //{
            // Logger.LogException(ex);
            //}

            try
            {
                Logger.Log("Kingshome Started");
                new Kingshome_IntegrationShoper();
                Logger.Log("Kingshome Ended");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
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
