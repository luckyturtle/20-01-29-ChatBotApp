/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Extensions;
using Link.Models;
using Link.Models.CommentModels;
using Link.Models.NotificationModels;
using Link.Models.NotificationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Link.Controllers.Api
{
    [Produces("application/json")]
    [Authorize]

    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly int NumberOfLoadingItems = 5;// load 5 items
        private readonly UserManager<ApplicationUser> _userManager;
        private const int maxNotificationLength = 78;//78 characters

        public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Notifications/5
        [HttpGet]
        [Route("api/[controller]/{type}/{index}")]
        public IEnumerable<NotificationViewModel> GetNotifications(int type, int index)
        {
            string currentUserId = _userManager.GetUserId(User);

            if (index >= 0)
            {
                if (type == NotificationType.All)
                {
                    return GetGeneralNotificatios(index, currentUserId);
                }
                else if (type == NotificationType.NewChatMessage)
                    //case: chat session, we should take into account the video notification as well.
                {
                    return GetChatVideoCallNotifications(index, currentUserId);
                }
                else
                {
                    return GetNotificationBySpesificType(type, index, currentUserId);

                }
            }

            return new List<NotificationViewModel>();
        }

        private IEnumerable<NotificationViewModel> GetNotificationBySpesificType(int type, int index, string currentUserId)
        {
            var notifications = _context.Notification
                                                    .Where(m => m.ToUserId == currentUserId && m.NotificationTypeId == type)
                                                    .OrderBy(c => c.IsSeen).ThenByDescending(c => c.CreatedDate)
                                                    .Skip(index)
                                                    .Take(NumberOfLoadingItems);

            var result = (from notification in notifications
                          join @group in _context.Group on notification.GroupId equals @group.Id
                          join user in _context.Users on notification.FromUserId equals user.Id
                          select new NotificationViewModel
                          {
                              Content = notification.Content.Length > maxNotificationLength ? string.Format("{0} ...", notification.Content.TruncateLongString(maxNotificationLength)) : notification.Content ?? "",
                              TimePassedMessage = Code.TimePassed(DateTime.Now, notification.CreatedDate),
                              GroupId = notification.GroupId,
                              Title = notification.Title,
                              To = notification.ToUserId,
                              Type = notification.NotificationTypeId,
                              From = notification.FromUserId,
                              FromName = user.UserName,
                              PhotoIsExist = user.Avatar != null,
                              GroupName = @group.Name,
                              IsSeen = notification.IsSeen,
                              CommentId = notification.CommentId,
                              CommentGroupType = CommentGroupType.GroupChat,
                              CreatedDate = notification.CreatedDate

                          }).ToList().Union(from notification in notifications.Where(o => Code.GenerateProfileGroupId(currentUserId, o.FromUserId) == o.GroupId)
                                            join user in _context.Users on notification.FromUserId equals user.Id
                                            select new NotificationViewModel
                                            {
                                                Content = notification.Content.Length > maxNotificationLength ? string.Format("{0} ...", notification.Content.TruncateLongString(maxNotificationLength)) : notification.Content ?? "",
                                                TimePassedMessage = Code.TimePassed(DateTime.Now, notification.CreatedDate),
                                                GroupId = notification.GroupId,
                                                Title = notification.Title,
                                                To = notification.ToUserId,
                                                Type = notification.NotificationTypeId,
                                                From = notification.FromUserId,
                                                FromName = user.UserName,
                                                PhotoIsExist = user.Avatar != null,
                                                GroupName = user.UserName,
                                                IsSeen = notification.IsSeen,
                                                CommentId = notification.CommentId,
                                                CommentGroupType = CommentGroupType.ProfileChat,
                                                CreatedDate = notification.CreatedDate
                                            });


            return result.OrderByDescending(c => c.CreatedDate);
        }

        private IEnumerable<NotificationViewModel> GetChatVideoCallNotifications(int index, string currentUserId)
        {
            var notifications = _context.Notification.Where(m => m.ToUserId == currentUserId && (m.NotificationTypeId == NotificationType.NewChatMessage || m.NotificationTypeId == NotificationType.VideoCall)).OrderBy(c => c.IsSeen).ThenByDescending(c => c.CreatedDate).Skip(index).Take(NumberOfLoadingItems);

            var result = (from notification in notifications
                          join @group in _context.Group on notification.GroupId equals @group.Id
                          join user in _context.Users on notification.FromUserId equals user.Id
                          select new NotificationViewModel
                          {
                              Content = notification.Content.Length > maxNotificationLength ? string.Format("{0} ...", notification.Content.TruncateLongString(maxNotificationLength)) : notification.Content ?? "",
                              TimePassedMessage = Code.TimePassed(DateTime.Now, notification.CreatedDate),
                              GroupId = notification.GroupId,
                              Title = notification.Title,
                              To = notification.ToUserId,
                              Type = notification.NotificationTypeId,
                              From = notification.FromUserId,
                              FromName = user.UserName,
                              PhotoIsExist = user.Avatar != null,
                              GroupName = @group.Name,
                              IsSeen = notification.IsSeen,
                              CommentId = notification.CommentId,
                              CommentGroupType = CommentGroupType.GroupChat,
                              CreatedDate = notification.CreatedDate

                          }).ToList().Union(from notification in notifications.Where(o => Code.GenerateProfileGroupId(currentUserId, o.FromUserId) == o.GroupId)
                                            join user in _context.Users on notification.FromUserId equals user.Id
                                            select new NotificationViewModel
                                            {
                                                Content = notification.Content.Length > maxNotificationLength ? string.Format("{0} ...", notification.Content.TruncateLongString(maxNotificationLength)) : notification.Content ?? "",
                                                TimePassedMessage = Code.TimePassed(DateTime.Now, notification.CreatedDate),
                                                GroupId = notification.GroupId,
                                                Title = notification.Title,
                                                To = notification.ToUserId,
                                                Type = notification.NotificationTypeId,
                                                From = notification.FromUserId,
                                                FromName = user.UserName,
                                                PhotoIsExist = user.Avatar != null,
                                                GroupName = user.UserName,
                                                IsSeen = notification.IsSeen,
                                                CommentId = notification.CommentId,
                                                CommentGroupType = CommentGroupType.ProfileChat,
                                                CreatedDate = notification.CreatedDate
                                            });


            return result.OrderByDescending(c => c.CreatedDate);
        }

        private IEnumerable<NotificationViewModel> GetGeneralNotificatios(int index, string currentUserId)
        {
            var currentUserNotifications = _context.Notification
                                                  .Where(m => m.ToUserId == currentUserId && (m.NotificationTypeId != NotificationType.NewChatMessage && m.NotificationTypeId != NotificationType.VideoCall))
                                                  .OrderBy(c => c.IsSeen).ThenByDescending(c => c.CreatedDate)
                                                  .Skip(index)
                                                  .Take(NumberOfLoadingItems);

            var result = (from notification in currentUserNotifications
                          join @group in _context.Group on notification.GroupId equals @group.Id//get group notifications
                          join user in _context.Users on notification.FromUserId equals user.Id
                          select new NotificationViewModel
                          {
                              Content = notification.Content.Length > maxNotificationLength ? string.Format("{0} ...", notification.Content.TruncateLongString(maxNotificationLength)) : notification.Content ?? "",
                              TimePassedMessage = Code.TimePassed(DateTime.Now, notification.CreatedDate),
                              GroupId = notification.GroupId,
                              Title = notification.Title,
                              To = notification.ToUserId,
                              Type = notification.NotificationTypeId,
                              From = notification.FromUserId,
                              FromName = user.UserName,
                              PhotoIsExist = user.Avatar != null,
                              GroupName = @group.Name,
                              IsSeen = notification.IsSeen,
                              CommentId = notification.CommentId,
                              CommentGroupType = CommentGroupType.Group,
                              CreatedDate = notification.CreatedDate
                          }).ToList().Union(from notification in currentUserNotifications
                                            join profile in _context.Users on notification.FromUserId equals profile.Id //get follower notifications
                                            select new NotificationViewModel
                                            {
                                                Content = notification.Content.Length > maxNotificationLength ? string.Format("{0} ...", notification.Content.TruncateLongString(maxNotificationLength)) : notification.Content ?? "",
                                                TimePassedMessage = Code.TimePassed(DateTime.Now, notification.CreatedDate),
                                                GroupId = notification.GroupId,
                                                Title = notification.Title,
                                                To = notification.ToUserId,
                                                Type = notification.NotificationTypeId,
                                                From = notification.FromUserId,
                                                FromName = profile.UserName,
                                                PhotoIsExist = profile.Avatar != null,
                                                GroupName = profile.UserName,
                                                IsSeen = notification.IsSeen,
                                                CommentId = notification.CommentId,
                                                CommentGroupType = CommentGroupType.Profile,
                                                CreatedDate = notification.CreatedDate

                                            });

            return result.OrderByDescending(c => c.CreatedDate);
        }

        [HttpGet]
        [Route("api/[controller]/notificationsNo/{type}")]
        public async Task<int> GetNotificationsNoAsync(int type)
        {
            string currentUserId = _userManager.GetUserId(User);
            if (type == NotificationType.All)//get all notification except chat
            {
                return await _context.Notification.CountAsync(m => m.ToUserId == currentUserId && (m.NotificationTypeId != NotificationType.NewChatMessage && m.NotificationTypeId != NotificationType.VideoCall) && m.IsSeen == false);
            }
            if (type == NotificationType.NewChatMessage)//case: chat session, we should take into account the video notification as well.
            {
                return await _context.Notification.CountAsync(m => m.ToUserId == currentUserId && (m.NotificationTypeId == NotificationType.NewChatMessage || m.NotificationTypeId == NotificationType.VideoCall) && m.IsSeen == false);
            }
            else
            {
                return await _context.Notification.CountAsync(m => m.ToUserId == currentUserId && m.NotificationTypeId == type && m.IsSeen == false);
            }
        }

        [HttpPut]
        [Route("api/[controller]/setNotificationAsRead/{id}/{type}")]
        public async Task<IActionResult> PutNotificationAsRead(string id, int type)
        {
            string currentUserId = _userManager.GetUserId(User);

            if (type == NotificationType.NewChatMessage)//case: chat session, we should take into account the video notification as well.
            {
                await _context.Notification.Where(m => m.ToUserId == currentUserId && m.GroupId == id && (m.NotificationTypeId == type || m.NotificationTypeId == NotificationType.VideoCall) && m.IsSeen == false).ForEachAsync(o => o.IsSeen = true);
            }
            else
            {
                await _context.Notification.Where(m => m.ToUserId == currentUserId && m.GroupId == id && m.NotificationTypeId == type && m.IsSeen == false).ForEachAsync(o => o.IsSeen = true);

            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool NotificationExists(string id)
        {
            return _context.Notification.Any(e => e.Id == id);
        }
    }
}