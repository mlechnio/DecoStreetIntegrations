﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DecoStreetIntegracja.Utils
{
    public class EmailSender
    {
        public void SendLogs()
        {
            SendEmail(string.Join("<br>", Logger.Events), "Logi", "mariusz.lechnio@gmail.com");

            if (Logger.NewProducts.Any())
            {
                SendEmail(string.Join("<br>", Logger.NewProducts), "Dodano nowe produkty", "mariusz.lechnio@gmail.com");
            }

            if (Logger.Exceptions.Any())
            {
                SendEmail(string.Join("<br>", Logger.Exceptions), "Wystąpił wyjątek", "mariusz.lechnio@gmail.com");
            }
        }

        private void SendEmail(string body, string subject, string recipient)
        {
            try
            {
                var message = new MailMessage(new MailAddress("integrator@decostreetpl-53073.shoparena.pl", "Integrator Shoper"),
                    new MailAddress(recipient))
                {
                    Subject = $"Integrator Shoper - {subject}",
                    IsBodyHtml = true,
                    Body = body
                };

                var client = new SmtpClient("s.mail.dcsaas.net", 587)
                {
                    Credentials = new NetworkCredential("integrator@decostreetpl-53073.shoparena.pl", "RPbEi9FsZv9ALDu"),
                    EnableSsl = true
                };

                client.Send(message);
                client.Dispose();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
