/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/


namespace Link.Models.CommentModels
{
    public class VoteType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static readonly byte Like = 1;
        public static readonly byte Dislike = 2;
        public static readonly byte Love = 3;
    }
}
