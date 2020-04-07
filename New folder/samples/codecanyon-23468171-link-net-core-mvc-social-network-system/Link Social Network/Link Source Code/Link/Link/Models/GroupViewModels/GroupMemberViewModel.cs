/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using System;

namespace Link.Models.GroupViewModels
{
    public class GroupMemberViewModel
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime JoinDate { get; set; }

    }
}
