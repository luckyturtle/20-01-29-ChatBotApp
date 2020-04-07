
/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

namespace Link.Services
{
    public interface IEmailLogs
    {
        void Add(string email, string subject, string message, int emailType, bool isSent);
    }
}
