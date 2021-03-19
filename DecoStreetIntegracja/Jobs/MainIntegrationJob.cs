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
            try
            {
                new D2_Kwadrat_Integration();
            }
            catch (Exception)
            {

            }

            //try
            //{
            //    new Adansonia_Integration();
            //}
            //catch (Exception)
            //{

            //}

            //try
            //{
            //    new Adansonia2_Integration();
            //}
            //catch (Exception)
            //{

            //}

            try
            {
                new Malodesign_Integration();
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

            ////

            //try
            //{
            //    new ArtPol_Integration();
            //}
            //catch (Exception)
            //{

            //}

            //try
            //{
            //    new Amiou_Integration();
            //}
            //catch (Exception)
            //{

            //}

            //try
            //{
            //    new Aluro_Integration();
            //}
            //catch (Exception)
            //{

            //}

            //try
            //{
            //    new CustomForm_Integration();
            //}
            //catch (Exception)
            //{

            //}

            //try
            //{
            //    new Durbas_Integration();
            //}
            //catch (Exception)
            //{

            //}
        }
    }
}
