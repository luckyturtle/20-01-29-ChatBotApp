/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Models;
using Link.Models.ConnectionModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wangkanai.Detection;

namespace Link.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserAgent _useragent;

        public NotificationHub(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IPlatformResolver platformResolver)
        {
            _context = context;
            _userManager = userManager;
            _useragent = platformResolver.UserAgent;
        }

        public override Task OnConnectedAsync()
        {
            string currentUserId = _userManager.GetUserId(Context.User);
            string userAgent = _useragent?.ToString();

            var connections = _context.Connections.Where(c => c.UserId == currentUserId);
            _context.Connections.RemoveRange(connections);

            _context.Connections.Add(
              new Connection
              {
                  ConnectionID = Context.ConnectionId,
                  UserAgent = _useragent?.ToString(),
                  UserId = currentUserId
              });

            _context.SaveChanges();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string currentUserId = _userManager.GetUserId(Context.User);
            string userAgent = _useragent?.ToString();
            Connection connection = _context.Connections.SingleOrDefault(c => c.UserId == currentUserId);
            _context.Connections.Remove(connection);
            _context.SaveChanges();
            return base.OnDisconnectedAsync(exception);
        }
    }
}
