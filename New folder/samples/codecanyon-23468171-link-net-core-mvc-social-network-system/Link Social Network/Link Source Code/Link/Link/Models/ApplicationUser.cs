/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Models.UserProfileModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Link.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public byte[] Avatar { get; set; }
        public string SocialMedia { get; set; }
        public string Description { get; set; }
        public string Interests { get; set; }
        public int? GenderId { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public int NumberOfFollwer { get; set; }
        public int NumberOfViewer { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime RegistrationDate { get; set; }
    }
}
