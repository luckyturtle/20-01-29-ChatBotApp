/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Models;
using Link.Models.CommentModels;
using Link.Models.UserLogsModels;
using Link.Models.UserProfileModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Link.Areas.Admin.Controllers.Api
{
    [Produces("application/json")]
    [Authorize(Roles = "ADMIN")]

    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int NumberOfLoadingItems = 10;
        public StatisticsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Statistics
        [HttpGet]
        [Route("api/Statistics/topProfile")]
        public IEnumerable<object> GetTopProfile()
        {
            return _context.Users.OrderByDescending(o => o.NumberOfFollwer).Take(NumberOfLoadingItems).ToList().Select(o =>
                new
                {
                    o.Id,
                    o.UserName,
                    o.NumberOfFollwer,
                    o.NumberOfViewer,
                    isUserAvatarExist = o.Avatar != null
                });
        }

        [HttpGet]
        [Route("api/Statistics/topGroup")]
        public IEnumerable<object> GetTopGroup()
        {
            return _context.Group
                .OrderByDescending(o => o.NumberOfMember)
                .Take(NumberOfLoadingItems)
                .ToList()
                .Select(o =>
                new
                {
                    o.Id,
                    o.Name,
                    PhotoIsExist = o.Photo != null,
                    o.NumberOfMember,
                    o.NumberOfViewer
                });
        }

        [HttpGet]
        [Route("api/Statistics/topLoginDay")]
        public IEnumerable<object> GetTopLoginDay()
        {
            return _context.UserLogs
               .Where(o => o.ActionDate.Year == DateTime.Now.Year && o.UserActionId == UserAction.LogIn)
               .GroupBy(ul => ul.ActionDate.Date)
               .Select(ul => new
               {
                   Value = ul.Count(),
                   Date = ul.Key
               }).OrderBy(o => o.Date)
                 .ToList()
                .Select(o => new
                {

                    Month = DateTime.Parse(o.Date.ToShortDateString()).ToString("MMM"),
                    Day = o.Date.Day,
                    NumberOfLogin = o.Value
                });
        }

        [HttpGet]
        [Route("api/Statistics/userLoginPerYear")]
        public IEnumerable<object> GetUserLoginPerYear()
        {
            return _context.UserLogs
               .Where(o => o.UserActionId == UserAction.LogIn)
               .GroupBy(ul => ul.ActionDate.Date.Year)
               .Select(ul => new
               {
                   Value = ul.Count(),
                   Year = ul.Key
               }).OrderByDescending(o => o.Value)
                 .ToList()
                .Select(o => new
                {
                    o.Year,
                    NumberOfLogin = o.Value
                });
        }

        // GET: api/Statistics/TotalProfiles/
        [HttpGet]
        [Route("api/Statistics/totalProfiles")]
        public int GetTotalProfiles()
        {
            return _context.Users.Count();
        }

        // GET: api/Statistics/TotalGroup/
        [HttpGet]
        [Route("api/Statistics/totalGroup")]
        public int GetTotalGroup()
        {
            return _context.Group.Count();
        }

        // GET: api/Statistics/TotalPost/
        [HttpGet]
        [Route("api/Statistics/totalPost")]
        public int GetTotalPost()
        {
            //Get all comments are not chat comment and not reply
            return _context.Comment.Count(o => o.CommentGroupTypeId != CommentGroupType.ProfileChat && o.Root == null);
        }

        // GET: api/Statistics/TotalMale/
        [HttpGet]
        [Route("api/Statistics/totalMale")]
        public int GetTotalMale()
        {
            return _context.Users.Count(o => o.GenderId == Gender.Male);
        }

        // GET: api/Statistics/TotalFemale/
        [HttpGet]
        [Route("api/Statistics/totalFemale")]
        public int GetTotalFemale()
        {
            return _context.Users.Count(o => o.GenderId == Gender.Female);
        }

        // GET: api/Statistics/CurrentConnections/
        [HttpGet]
        [Route("api/Statistics/currentConnections")]
        public int GetCurrentConnections()
        {
            return _context.Connections.Count();
        }
    }
}
