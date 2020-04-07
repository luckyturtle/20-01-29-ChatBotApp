/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Models;
using Link.Models.UserLogsModels;
using Microsoft.AspNetCore.Identity;
using System;
using Wangkanai.Detection;

namespace Link.Services
{
    public class UserLog : IUserLog
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserAgent _useragent;

        public UserLog(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IPlatformResolver platformResolver)
        {
            _context = context;
            _userManager = userManager;
            _useragent = platformResolver.UserAgent;
        }

        public void Add(byte userAction, string userId)
        {
            var userLogs = new UserLogs()
            {
                ActionDate = DateTime.Now,
                UserAgent = _useragent?.ToString(),
                UserId = userId,
                UserActionId = userAction 
            };

            _context.Add(userLogs);
            _context.SaveChanges();
        }
    }
}
