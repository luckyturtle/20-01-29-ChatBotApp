/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

namespace Link.Models.UserLogsModels
{
    public class UserAction
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static readonly byte LogOut = 0;
        public static readonly byte LogIn = 1;
    }
}
