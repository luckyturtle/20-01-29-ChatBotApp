/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using System;

namespace Link.Models.GroupModels
{
    public class GroupMemberRequest
    {
        public string Id { get; set; }

        public string GroupId { get; set; }

        public string FromId { get; set; }

        public string ToEmail { get; set; }

        public string Code { get; set; }

        public DateTime RequestDate { get; set; }
    }
}
