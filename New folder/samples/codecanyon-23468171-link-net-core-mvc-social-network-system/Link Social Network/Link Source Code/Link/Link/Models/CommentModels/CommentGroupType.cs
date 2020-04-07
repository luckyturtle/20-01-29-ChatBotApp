/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/


namespace Link.Models.CommentModels
{
    public class CommentGroupType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static readonly byte All = 0;
        public static readonly byte Profile = 1;
        public static readonly byte Group = 2;
        public static readonly byte Page = 3;
        public static readonly byte GroupChat = 4;
        public static readonly byte ProfileChat = 5;

    }
}
