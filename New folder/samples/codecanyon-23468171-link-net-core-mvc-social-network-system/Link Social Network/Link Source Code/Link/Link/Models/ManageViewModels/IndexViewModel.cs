
using Link.Models.UserProfileModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Link.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }

        public byte[] Avatar { get; set; }

        [Display(Name = "Social Media Links")]
        public string SocialMedia { get; set; }
        public string Description { get; set; }
        public string Interests { get; set; }
        [Display(Name = "Gender")]
        public int? GenderId { get; set; }
        public Gender Gender { get; set; }
        [Display(Name = "Birthdate")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime? BirthDate { get; set; }
        public int NumberOfFollwer { get; set; }
        public int NumberOfViewer { get; set; }
        public bool IsCurrentUserFollowProfile { get; set; }
        public bool IsCurrentProfileForCurrentUser { get; set; }
    
    }
}
