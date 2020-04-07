/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;

namespace Link.Models.CommentModels
{

    public class Comment
    {
        [Required]
        public string Id { get; set; }
        public string Parent { get; set; }
        public string Root { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string FileURL { get; set; }

        public string FileMimeType { get; set; }

        public string Content { get; set; }

        public string Pings { get; set; }

        public string Creator { get; set; }

        public bool CreatedByAdmin { get; set; }

        public bool CreatedByCurrentUser { get; set; }

        public int UpvoteCount { get; set; }

        public bool UserHasUpvoted { get; set; }

        public string UserVoted { get; set; }

        public bool IsNew { get; set; }

        public byte[] File { get; set; }

        public string GroupId { get; set; }

        public int? CommentGroupTypeId { get; set; }

        public CommentGroupType CommentGroupType { get; set; }
    }
}
