using DecoStreetIntegracja.Integrations;
using DecoStreetIntegracja.Integrations.ArtPol;
using DecoStreetIntegracja.Utils;
using Quartz;
using System;
using System.Diagnostics;

namespace DecoStreetIntegracja.Jobs
{
    public class MainIntegrationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var watch = Stopwatch.StartNew();
            Logger.Clear();
            Logger.Log("Integrations Started");
            RunIntegrations();
            watch.Stop();
            var timespan = new TimeSpan(watch.ElapsedTicks);
            Logger.Log("Integrations Ended");
            Logger.LogFirst($"Execution Time: {timespan}");
            new EmailSender().SendLogs();
        }

        public void RunIntegrations()
        {
            //RunSingle();
            RunShoper();
        }
        private void RunSingle()
        {

            try
            {
                Logger.Log("ArtPol RC Part 2 Started");
                //new ArtPol_IntegrationShoperRMNEW();
                //new ArtPol_IntegrationShoperRMABSP();
                //new ArtPol_IntegrationShoperRMCIT();
                //new ArtPol_IntegrationShoperRMPLT();
                //new ArtPol_IntegrationShoperRMFTH();
                //new ArtPol_IntegrationShoperRMABSG();
                //new ArtPol_IntegrationShoperRMABSO();
                //new ArtPol_IntegrationShoperRMBAL();
                //new ArtPol_IntegrationShoperRMMAR();
                //new ArtPol_IntegrationShoperRMKLM();
                //new ArtPol_IntegrationShoperRMMTS();
                //new ArtPol_IntegrationShoperRMVVG();
                //new ArtPol_IntegrationShoperRMFLB();
                //new ArtPol_IntegrationShoperRMFLI();
                //new ArtPol_IntegrationShoperRMFLR();
                //new ArtPol_IntegrationShoperRMFLN();
                //new ArtPol_IntegrationShoperRMLNDN();
                //new ArtPol_IntegrationShoperRMLNDT();
                //new ArtPol_IntegrationShoperRMLAV();
                //new ArtPol_IntegrationShoperRMFIG();
                //new ArtPol_IntegrationShoperRMPET();

                //new ArtPol_IntegrationShoperRCNEW();
                //new ArtPol_IntegrationShoperRCABSP();
                //new ArtPol_IntegrationShoperRCABSG();
                //new ArtPol_IntegrationShoperRCABSO();
                //new ArtPol_IntegrationShoperRCANT();
                //new ArtPol_IntegrationShoperRCINN();
                //new ArtPol_IntegrationShoperRCNY();
                //new ArtPol_IntegrationShoperRCKID();
                //new ArtPol_IntegrationShoperRCPLN();
                //new ArtPol_IntegrationShoperRCPOS();
                //new ArtPol_IntegrationShoperRCKLM();
                //new ArtPol_IntegrationShoperRCMTS();
                //new ArtPol_IntegrationShoperRCBIN();
                //new ArtPol_IntegrationShoperRCBII();
                //new ArtPol_IntegrationShoperRCLND();
                //new ArtPol_IntegrationShoperRCFIG();
                //new ArtPol_IntegrationShoperRCINNA();
                //new ArtPol_IntegrationShoperRCCAC();
                //new ArtPol_IntegrationShoperRCTRP();
                //new ArtPol_IntegrationShoperRCANI();
                //new ArtPol_IntegrationShoperRCHRS();
                //new ArtPol_IntegrationShoperRCBFL();
                //new ArtPol_IntegrationShoperRCPET();
                //new ArtPol_IntegrationShoperRCBRD();

                Logger.Log("ArtPol RC Part 2 Ended");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            //try
            //{
            //    Logger.Log("Emibig_IntegrationShoper Started");
            //    new Emibig_IntegrationShoper();
            //    Logger.Log("Emibig_IntegrationShoper Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}

            //try
            //{
            //    Logger.Log("Moosee Started");
            //    new Moosee_IntegrationShoper();
            //    Logger.Log("Moosee Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}

            //try
            //{
            //    Logger.Log("Atos Started");
            //    new Atos_IntegrationShoper();
            //    Logger.Log("Atos Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}

            //try
            //{
            //    Logger.Log("Emibig Started");
            //    new Emibig_IntegrationShoper();
            //    Logger.Log("Emibig Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}


            //try
            //{
            //    Logger.Log("Malodesign Started");
            //    new Malodesign_IntegrationShoper();
            //    Logger.Log("Malodesign Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}

            //try
            //{
            //    Logger.Log("Kingshome Started");
            //    new Kingshome_IntegrationShoper();
            //    Logger.Log("Kingshome Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}

            //try
            //{
            //    Logger.Log("D2_Kwadrat Started");
            //    new D2_Kwadrat_IntegratorShoper();
            //    Logger.Log("D2_Kwadrat Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}

            //try
            //{
            //    Logger.Log("Eltap_IntegrationShoper Started");
            //    new Eltap_IntegrationShoper();
            //    Logger.Log("Eltap_IntegrationShoper Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}

            //try
            //{
            //    Logger.Log("Emibig_IntegrationShoper Started");
            //    new Emibig_IntegrationShoperUpdateImages();
            //    Logger.Log("Emibig_IntegrationShoper Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}
        }

        private void RunShoper()
        {
            try
            {
                Logger.Log("D2_Kwadrat Started");
                new D2_Kwadrat_IntegratorShoper();
                Logger.Log("D2_Kwadrat Ended");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            try
            {
                Logger.Log("Moosee Started");
                new Moosee_IntegrationShoper();
                Logger.Log("Moosee Ended");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

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

            //try
            //{
            //    Logger.Log("Malodesign Started");
            //    new Malodesign_IntegrationShoper();
            //    Logger.Log("Malodesign Ended");
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}
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
