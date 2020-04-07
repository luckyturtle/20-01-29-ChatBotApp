/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Link.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration Configuration;

        public EmailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                string userName = Configuration.GetValue<string>("EmailSettings:UserName");
                string passwrod = Configuration.GetValue<string>("EmailSettings:Password");
                var mailMessage = new MailMessage(userName, email, subject, message)
                {
                    IsBodyHtml = Configuration.GetValue<bool>("EmailSettings:IsBodyHtml")
                };

                var networkCredential = new NetworkCredential(userName, passwrod);
                var mailClient = new SmtpClient
                {
                    EnableSsl = Configuration.GetValue<bool>("EmailSettings:EnableSsl"),
                    Host = Configuration.GetValue<string>("EmailSettings:SmtpServer"),
                    Port = Configuration.GetValue<int>("EmailSettings:Port"),
                    Credentials = networkCredential
                };

                mailClient.Send(mailMessage);
            }
            catch { }
            return Task.CompletedTask;
        }

        public Task SendEmailAsync(List<string> emailList, string subject, string message)
        {
            try
            {
                string userName = Configuration.GetValue<string>("EmailSettings:UserName");
                string passwrod = Configuration.GetValue<string>("EmailSettings:Password");
                var combined = string.Join(",", emailList);

                var mailMessage = new MailMessage(userName, combined, subject, message)
                {
                    IsBodyHtml = Configuration.GetValue<bool>("EmailSettings:IsBodyHtml")
                };

                var networkCredential = new NetworkCredential(userName, passwrod);
                var mailClient = new SmtpClient
                {
                    EnableSsl = Configuration.GetValue<bool>("EmailSettings:EnableSsl"),
                    Host = Configuration.GetValue<string>("EmailSettings:SmtpServer"),
                    Port = Configuration.GetValue<int>("EmailSettings:Port"),
                    Credentials = networkCredential
                };


                mailClient.Send(mailMessage);
            }
            catch { }
            return Task.CompletedTask;
        }

    }
}
