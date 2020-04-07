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
using Link.Models.ManageViewModels;
using Link.Models.PeopleViewModels;
using Link.Models.UserProfileModels;
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

    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly int NumberOfLoadingItems = 8;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;

        }

        // GET: api/Profile
        [HttpGet]
        [Route("api/[controller]/UserName/{name}")]
        public IEnumerable<IndexViewModel> GetByName(string name)
        {
            var userViewModelList = new List<IndexViewModel>();
            var userList = _context.Users.Where(u => u.UserName.ToLower().Contains(name));

            foreach (var user in userList)
            {
                var model = new IndexViewModel
                {
                    Username = user.UserName,
                    UserId = user.Id
                };

                userViewModelList.Add(model);
            }

            return userViewModelList;
        }

        // GET: api/Profile/5
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var user = await _context.Users.SingleAsync(u => u.Id == id);
            if (user == null)
            {
                return BadRequest();
            }

            var model = new IndexViewModel
            {
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
                SocialMedia = user.SocialMedia
            };

            return Ok(model);
        }

        [HttpGet]
        [Route("api/[controller]/Friends/{name}/{index}")]
        public IEnumerable<PeopleViewModel> GetFriends(string name, int index)
        {
            var currentUserId = _userManager.GetUserId(User);
            //
            //if two users follow each other, then they are a friend
            var friends = _context.Followers
                                 .Where(f => f.FollowersId == currentUserId)
                                 .Select(o => new { FriendId = o.UserId })
                                 .Intersect(_context.Followers.Where(f => f.UserId == currentUserId)
                                 .Select(o => new { FriendId = o.FollowersId }))
                                 .Join(_context.Users, f => f.FriendId, u => u.Id,
                                (followers, users) => new PeopleViewModel
                                {
                                    Id = users.Id,
                                    Name = users.UserName,
                                    PhotoIsExist = users.Avatar != null,
                                    IsCurrentUserFollowUser = _context.Followers
                                                                   .Any(f => f.FollowersId == currentUserId
                                                                   && f.UserId == users.Id),
                                    IsUserFollowCurrentUser = _context.Followers
                                                                    .Any(f => f.UserId == currentUserId
                                                                    && f.FollowersId == users.Id),
                                    NumberOfFollwer = users.NumberOfFollwer,
                                    NumberOfViewer = users.NumberOfViewer,

                                }).Where(u => name == "*" ? 1 == 1 : u.Name.Contains(name))
                                  .Skip(index)
                                  .Take(NumberOfLoadingItems);



            return friends;
        }


        [HttpGet]
        [Route("api/[controller]/Friends/{index}")]
        public IEnumerable<FriendsViewModel> GetFriends(int index)
        {
            var currentUserId = _userManager.GetUserId(User);

            //if two users follow each other, then they are a friend
            var friends = _context.Followers.Where(f => f.FollowersId == currentUserId).Select(o => new { FriendId = o.UserId })
                                 .Intersect(_context.Followers.Where(f => f.UserId == currentUserId).Select(o => new { FriendId = o.FollowersId }))
                                .Join(_context.Users, f => f.FriendId, u => u.Id,
                                (followers, users) => new FriendsViewModel
                                {
                                    Id = users.Id,
                                    Name = users.UserName,
                                    PhotoIsExist = users.Avatar != null,
                                    GroupId = Code.GenerateProfileGroupId(currentUserId, users.Id),
                                    IsActive = false
                                }).Skip(index).Take(NumberOfLoadingItems).ToList();

            var currentActiveFriends = _context.Connections.Join(friends, c => c.UserId, f => f.Id,
                                                (connection, friend) =>
                                                new
                                                {
                                                    friend.Id
                                                }).ToList();

            for (int i = 0; i < friends.Count(); i++)
            {
                friends[i].IsActive = currentActiveFriends.Any(o => o.Id == friends[i].Id);
            }

            return friends;
        }

        // Get: api/Profile/IsCurrentUserFollow/{id}
        [Route("api/[controller]/IsCurrentUserFollow/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetIsCurrentUserFollow(string id)
        {
            var user = await _context.Users.SingleAsync(u => u.Id == id);
            var follower = _context.Followers.Single(u => u.FollowersId == id && u.UserId == user.Id);

            return Ok(follower == null);
        }

        [HttpGet]
        [Route("api/[controller]/numberOfFriends")]
        public async Task<int> GetNumberOfFriendsAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            return await _context.Followers.Where(f => f.FollowersId == currentUserId).Select(o => new { FriendId = o.UserId })
                                 .Intersect(_context.Followers.Where(f => f.UserId == currentUserId).Select(o => new { FriendId = o.FollowersId }))
                                 .CountAsync();


        }

        // PUT: api/Profile/Follow/{id}
        [Route("api/[controller]/Follow/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutFollow(string id)
        {
            var user = _context.Users.Single(u => u.Id == id);

            var numberOfFollwer = 0;
            var currentUserId = _userManager.GetUserId(User);
            var follower = _context.Followers.SingleOrDefault(u => u.FollowersId == currentUserId && u.UserId == user.Id);

            if (follower == null)//check if current user follow the curreent user before
            {
                user.NumberOfFollwer += 1;

                _context.Followers.Add(
                 new Followers
                 {
                     FollowersId = currentUserId,
                     UserId = user.Id,
                     FollowDate = DateTime.Now,
                     IsSeen = false

                 });


            }
            else
            {
                user.NumberOfFollwer -= 1;
                _context.Followers.Remove(follower);
            }

            numberOfFollwer = user.NumberOfFollwer;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //Send notification to user who current user has followed
            var connection = _context.Connections.SingleOrDefault(o => o.UserId == user.Id);
            if (connection != null)
            {
                var numberUserUnSeenNotifications = _context.Followers.Count(m => m.UserId == user.Id && m.IsSeen == false);

                await _hubContext.Clients.Client(connection.ConnectionID).SendAsync("RefreshFollowersNotificationNum", numberUserUnSeenNotifications);
            }
            return Ok(numberOfFollwer);
        }

        // PUT: api/Profile/Viewer/{id}
        [Route("api/[controller]/Viewer/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutViewer(string id)
        {
            var numberOfViewers = 0;
            var currentUserId = _userManager.GetUserId(User);
            var user = _context.Users.Single(u => u.Id == id);
            var viewer = _context.Viewers.SingleOrDefault(u => u.ViewersId == currentUserId && u.UserId == user.Id);

            if (viewer == null)//check if current user view the curreent user before
            {
                user.NumberOfViewer += 1;
                _context.Viewers.Add(new Viewers()
                {
                    ViewersId = currentUserId,
                    UserId = user.Id
                });
            }
            _context.Entry(user).State = EntityState.Modified;
            numberOfViewers = user.NumberOfViewer;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(numberOfViewers);
        }

        private bool ProfileExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
