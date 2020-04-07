/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/


namespace Link.Models.UserProfileModels
{
    public class Gender
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }

        public static readonly int Male = 1;
        public static readonly int Female = 2;


    }
}
