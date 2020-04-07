/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Link.Models.GroupModels
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }
        public byte[] Photo { get; set; }

        [Required]
        public int GroupTypeId { get; set; }
        public GroupType GroupType { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Creator { get; set; }
        public string LastUpdatedBy { get; set; }
        public int NumberOfViewer { get; set; }
        public int NumberOfMember { get; set; }

    }
}
