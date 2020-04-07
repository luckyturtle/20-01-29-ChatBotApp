/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

namespace Link.Models.UserProfileViewModels
{
    public class FollowersViewModel
    {
        public string FollowersId { get; set; }

        public bool PhotoIsExist { get; set; }

        public string FollowersName { get; set; }

        public bool IsSeen { get; set; }

        public string TimePassedMessage { get; set; }

        public int? GenderId { get; set; }

    }
}
