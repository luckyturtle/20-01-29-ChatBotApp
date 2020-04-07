/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Models;
using Link.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;

namespace Link.Controllers
{
    [Authorize]

    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProfileController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }


        public IActionResult GetUserProfile(string id)
        {
            var user = _context.Users.SingleOrDefault(m => m.Id == id);

            var file = user.Avatar;

            string webRootPath = _hostingEnvironment.WebRootPath;

            if (file == null)
            {
                file = System.IO.File.ReadAllBytes(Path.Combine(webRootPath, "images", "avatar.png"));
            }

            return File(file, MediaTypeNames.Application.Octet, user.UserName);

        }

        [Route("Profile/friends")]
        public IActionResult Friends()
        {
            return View();
        }

        [HttpGet]
        [Route("Profile/{name}")]
        [Route("Profile/Index/{name}")]
        public IActionResult Index(string name)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == name);
            var currentUserId = _userManager.GetUserId(User);

            if (user == null)
            {
                return BadRequest();
            }

            var isCurrentUserFollowProfile = _context.Followers.Any(u => u.FollowersId == currentUserId && u.UserId == user.Id);

            var model = new IndexViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                Avatar = user.Avatar,
                BirthDate = user.BirthDate,
                Description = user.Description,
                GenderId = user.GenderId,
                Interests = user.Interests,
                NumberOfFollwer = user.NumberOfFollwer,
                NumberOfViewer = user.NumberOfViewer,
                SocialMedia = user.SocialMedia,
                IsCurrentUserFollowProfile = isCurrentUserFollowProfile,
                IsCurrentProfileForCurrentUser = (currentUserId == user.Id)
            };

            //set the current user seen the selected user profile request
            if (!model.IsCurrentProfileForCurrentUser)
            {
                var seenStatus = _context.Followers.SingleOrDefault(f => f.FollowersId == model.UserId && f.UserId == currentUserId);
                if (seenStatus != null)
                {
                    seenStatus.IsSeen = true;
                    seenStatus.SeenDate = DateTime.Now;
                    _context.SaveChanges();
                }
            }

            return View(model);
        }
    }
}