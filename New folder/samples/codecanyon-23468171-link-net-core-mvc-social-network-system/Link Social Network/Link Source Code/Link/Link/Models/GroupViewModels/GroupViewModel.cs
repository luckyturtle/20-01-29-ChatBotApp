/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Link.Models.GroupViewModels
{
    public class GroupViewModel
    {

        public string Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }
        public IFormFile Photo { get; set; }
        public byte[] PhotoStream { get; set; }
        [Required]
        public int GroupTypeId { get; set; }
        //public GroupType GroupType { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Creator { get; set; }
        public string LastUpdatedBy { get; set; }
        public int NumberOfViewer { get; set; }
        public int NumberOfMember { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsCurrentUserFollowGroup { get; set; }
        public bool PhotoIsExist { get; set; }
    }
}
