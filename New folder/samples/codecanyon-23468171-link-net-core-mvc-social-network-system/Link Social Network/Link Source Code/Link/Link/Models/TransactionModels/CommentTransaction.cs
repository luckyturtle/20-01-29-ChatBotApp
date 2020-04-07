/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Link.Models.TransactionModels
{
    public class CommentTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string CommentRoot { get; set; }
        public string CommentId { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public int CommentTransactionTypeId { get; set; }
        public CommentTransactionType CommentTransactionType { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Data { get; set; }
    }
}
