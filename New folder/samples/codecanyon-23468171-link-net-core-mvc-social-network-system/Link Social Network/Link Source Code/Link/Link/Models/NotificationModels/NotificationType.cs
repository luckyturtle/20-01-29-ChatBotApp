/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

namespace Link.Models.NotificationModels
{
    public class NotificationType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static readonly byte All = 0;
        public static readonly byte NewChatMessage = 1;
        public static readonly byte System = 2;
        public static readonly byte GroupRequest = 3;
        public static readonly byte AddNewComment = 4;
        public static readonly byte AddReplyOnComment = 5;
        public static readonly byte VoteComment = 6;
        public static readonly byte FriendRequest = 7;
        public static readonly byte VideoCall = 8;

    }
}
