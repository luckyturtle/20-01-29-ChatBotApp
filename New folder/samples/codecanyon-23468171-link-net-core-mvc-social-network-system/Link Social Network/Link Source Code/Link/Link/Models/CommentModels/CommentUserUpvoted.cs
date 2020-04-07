/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Link.Models.CommentModels
{
    /// <summary>
    /// This class describ the user who apply the upvoted action per comment.
    /// </summary>
    public class CommentUserVote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string CommentId { get; set; }
        public string UserId { get; set; }

        public int VoteTypeId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
