/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

namespace Link.Models.PeopleViewModels
{
    public class PeopleViewModel
    {
        public string Id { get; set; }
        public bool PhotoIsExist { get; set; }
        public string Name { get; set; }
        public int NumberOfViewer { get; set; }
        public int NumberOfFollwer { get; set; }
        public bool IsCurrentUserFollowUser { get; set; }
        public bool IsUserFollowCurrentUser { get; set; }
    }
}