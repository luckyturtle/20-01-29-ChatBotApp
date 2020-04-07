/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/


namespace Link.Models.EmailLogsModels
{
    public class EmailType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static readonly byte Registration = 1;
        public static readonly byte ForgotPassword = 2;
        public static readonly byte System = 3;
        public static readonly byte GroupRequest = 4;
        public static readonly byte AddNewComment = 5;
        public static readonly byte AddReplyOnComment = 6;
        public static readonly byte VoteComment = 7;
    }
}
