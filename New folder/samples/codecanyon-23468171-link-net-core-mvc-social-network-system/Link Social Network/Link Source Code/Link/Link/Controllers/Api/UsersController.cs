/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Models;
using Link.Models.CommentModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Link.Controllers.Api
{
    [Produces("application/json")]
    [Authorize]

    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: api/Users
        [HttpGet]
        [Route("api/[controller]/")]
        public IEnumerable<CommentUserProfileVIew> GetUsers()
        {
            List<CommentUserProfileVIew> CommentUserProfile = new List<CommentUserProfileVIew>();
            foreach (var user in _context.Users)
            {
                CommentUserProfile.Add(new CommentUserProfileVIew()
                {
                    email = user.Email,
                    fullname = user.Email,
                    id = user.Id,
                    profile_picture_url = ""
                });
            }
            return CommentUserProfile;
        }




    }
}
