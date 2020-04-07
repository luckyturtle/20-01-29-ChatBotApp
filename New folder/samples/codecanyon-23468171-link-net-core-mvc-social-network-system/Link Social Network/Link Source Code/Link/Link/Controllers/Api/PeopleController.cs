/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Hubs;
using Link.Models;
using Link.Models.PeopleViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Link.Controllers.Api
{
    [Produces("application/json")]
    [Authorize]

    public class PeopleController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly int NumberOfLoadingItems = 8;
        public PeopleController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("api/[controller]/numberOfPeople")]
        public async Task<int> GetNumberOfPeopleAsync()
        {
            return await _context.Users.CountAsync();
        }

        [HttpGet]
        [Route("api/[controller]/{name}/{index}")]
        public IEnumerable<PeopleViewModel> GetPeople(string name, int index)
        {

            var currentUserId = _userManager.GetUserId(User);
            var peopleViewModelList = new List<PeopleViewModel>();

            var people = _context.Users
                              .Where(u => u.Id != currentUserId &&
                              (name == "*" ? 1 == 1 : u.UserName.Contains(name)))
                              .Select(user => new PeopleViewModel
                              {
                                  Id = user.Id,
                                  IsCurrentUserFollowUser = _context.Followers
                                                                   .Any(f => f.FollowersId == currentUserId
                                                                   && f.UserId == user.Id),
                                  IsUserFollowCurrentUser = _context.Followers
                                                                    .Any(f => f.UserId == currentUserId
                                                                    && f.FollowersId == user.Id),
                                  Name = user.UserName,
                                  NumberOfFollwer = user.NumberOfFollwer,
                                  NumberOfViewer = user.NumberOfViewer,
                                  PhotoIsExist = user.Avatar != null
                              })
                             .OrderByDescending(o => o.IsCurrentUserFollowUser)
                             //.ThenBy(o => o.IsUserFollowCurrentUser)
                             .ThenBy(o => o.Name)
                             .Skip(index)
                             .Take(NumberOfLoadingItems);
            return people;
        }
    }
}