/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Extensions;
using Link.Hubs;
using Link.Models;
using Link.Models.UserProfileViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Link.Controllers.Api
{
    [Produces("application/json")]
    [Authorize]

    public class FollowersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public FollowersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("api/[controller]/followersNo")]
        public async Task<int> GetFollowersNoAsync(int type)
        {
            string currentUserId = _userManager.GetUserId(User);
            return await _context.Followers.CountAsync(m => m.UserId == currentUserId && m.IsSeen == false);
        }

        // GET: api/Followers
        [HttpGet]
        [Route("api/[controller]/{index}")]
        public IEnumerable<FollowersViewModel> GetFollowers(int index)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (index >= 0)
            {
                var currentUserFollowers = _context.Followers.Where(f => f.UserId == currentUserId).OrderByDescending(o => o.FollowDate).Skip(index).Take(5)
                                                   .Join(_context.Users, f => f.FollowersId, u => u.Id, (follower, user) =>
                                                          new FollowersViewModel
                                                          {
                                                              FollowersId = follower.FollowersId,
                                                              FollowersName = user.UserName,
                                                              PhotoIsExist = user.Avatar != null,
                                                              TimePassedMessage = Code.TimePassed(DateTime.Now, follower.FollowDate),
                                                              IsSeen = follower.IsSeen,
                                                              GenderId = user.GenderId
                                                          }).ToList();

                RefreshCurrentUserFollowersNotifications(currentUserId);

                return currentUserFollowers;
            }

            return new List<FollowersViewModel>();
        }

        private void RefreshCurrentUserFollowersNotifications(string currentUserId)
        {
            var currentUserConnectionId = _context.Connections.SingleOrDefault(o => o.UserId == currentUserId);
            var numberOfCurrentUserUnSeenNotifications = _context.Followers.Count(m => m.UserId == currentUserId && m.IsSeen == false);
            if (currentUserConnectionId != null)
            {
                _hubContext.Clients.Client(currentUserConnectionId.ConnectionID).SendAsync("RefreshFollowersNotificationNum", numberOfCurrentUserUnSeenNotifications);
            }
        }
    }
}