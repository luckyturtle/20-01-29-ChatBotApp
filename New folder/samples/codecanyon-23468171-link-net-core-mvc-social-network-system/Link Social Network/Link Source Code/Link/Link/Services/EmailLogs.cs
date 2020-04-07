/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Models.EmailLogsModels;
using System;

namespace Link.Services
{
    public class EmailLogs : IEmailLogs
    {
        private readonly ApplicationDbContext _context;
        public EmailLogs(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(string email, string subject, string message, int emailType, bool isSent)
        {
            var newEmailLogInfo = new EmailLogsInfo()
            {
                CreatedDate = DateTime.Now,
                EmailTypeId = emailType,
                From = "",
                To = email,
                IsSent = isSent,
                Message = message,
                Subject = subject
            };

            _context.Add(newEmailLogInfo);
            _context.SaveChanges();
        }
    }
}
