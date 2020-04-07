/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

namespace Link.Models.TransactionModels
{
    public class CommentTransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static readonly byte Add = 1;
        public static readonly byte Edit = 2;
        public static readonly byte Delete = 3;
        public static readonly byte Vote = 4;
    }

}