/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using System;

namespace Link.Models.NotificationViewModels
{
    public class NotificationViewModel
    {
        public DateTime CreatedDate { get; set; }
        public string CommentId { get; set; }
        public int CommentGroupType { get; set; }

        public int Type { get; set; }
        public string Title { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string FromName { get; set; }
        public string Content { get; set; }
        public bool PhotoIsExist { get; set; }
        public string TimePassedMessage { get; set; }
        public bool IsSeen { get; set; }
    }
}
