using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Project.COMMON.Tools
{
    public static class MaiService
    {
        public static void Send(string receiver, string password = "atlkbjteiruyhww", string body = "Test mesajı.", string subject = "Email Testi", string sender = "yzl3157test@gmail.com")
        {
            MailAddress senderEmail = new MailAddress(sender);
            MailAddress receiverEmail = new MailAddress(receiver);

            // Bizim email işlemlerimiz smtp ye göre yapılır.

            SmtpClient smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };

            using (MailMessage message = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = subject,
                Body = body,
            })
            {
                smtpClient.Send(message);
            }




        }

    }
}
