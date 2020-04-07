/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/


namespace Link.Models.GroupModels
{
    public class GroupType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static readonly byte Public = 1;
        public static readonly byte Private = 2;

    }
}