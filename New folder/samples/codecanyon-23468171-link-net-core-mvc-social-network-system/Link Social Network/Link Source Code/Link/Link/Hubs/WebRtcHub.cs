/*****************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 * //https://github.com/mgiuliani/webrtc-video-chat/blob/master/VideoChat/Hubs/WebRtcHub.cs
******************************************************************************************************/
using Link.Data;
using Link.Extensions;
using Link.Models;
using Link.Models.CommentModels;
using Link.Models.NotificationModels;
using Link.Models.WebRtcUserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Link.Hubs
{
    public class WebRtcHub : Hub

    {
        private static readonly List<User> Users = new List<User>();
        private static readonly List<UserCall> UserCalls = new List<UserCall>();
        private static readonly List<CallOffer> CallOffers = new List<CallOffer>();

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public WebRtcHub(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;

        }


        public void Join(string username)
        {
            string currentUserId = _userManager.GetUserId(Context.User);

            // Add the new user
            Users.Add(new User
            {
                Id = currentUserId,
                Username = username,
                ConnectionId = Context.ConnectionId
            });
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Hang up any calls the user is in
            HangUp(); // Gets the user from "Context" which is available in the whole hub

            // Remove the user
            Users.RemoveAll(u => u.ConnectionId == Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }



        public void CallUser(string userName)
        {
            var callingUser = Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
            var targetUser = Users.SingleOrDefault(u => u.Username == userName);

            if (!Users.Any(u => u.Username == userName))//if user not connected
            {
                //Get target User id by name
                var targetUserDbInfo = _context.Users.SingleOrDefault(u => u.UserName == userName);
                if (targetUserDbInfo != null)
                {
                    // missed your video chat.
                    SendNotificationAboutMissCall(MissCallStatus.UserNotConnected, callingUser, targetUserDbInfo.Id);
                }

                // If not, let the caller know
                Clients.Caller.SendAsync("calleeNotConnected");
            }
            // Make sure the person we are trying to call is still here
            if (targetUser == null)
            {
                // If not, let the caller know
                Clients.Caller.SendAsync("callDeclined", "The user you called has left.");

                return;
            }

            // And that they aren't already in a call
            if (GetUserCall(targetUser.ConnectionId) != null)
            {
                Clients.Caller.SendAsync("callDeclined", $"{targetUser.Username} is already in a call.");

                return;
            }

            // They are here, so tell them someone wants to talk
            Clients.Client(targetUser.ConnectionId).SendAsync("incomingCall", callingUser);

            // Create an offer
            CallOffers.Add(new CallOffer
            {
                Caller = callingUser,
                Callee = targetUser
            });
        }

        //Case: user not connected to Link.

        private enum MissCallStatus
        {
            UserConnected,
            UserNotConnected
        }
        //Case: user is connected to Link. But not accept the call.
        private void SendNotificationAboutMissCall(MissCallStatus missCallStatus, User callingUser, string targetUserId)
        {
            string msg = "";
            switch (missCallStatus)
            {
                case MissCallStatus.UserConnected:
                    {
                        msg = $"Hi, I was trying to call you 📞 but can't get 🔌 ";
                    }
                    break;
                case MissCallStatus.UserNotConnected:
                    {
                        msg = $"Hi, I tried to call you 📞 but you didn't pick up 💤";

                    }
                    break;
            }
            // send notification for user, about missed video chat call.
            var notification = new Notification
            {
                Title = "Missed call reminder",
                Content = $"{callingUser.Username}, missed your video call",
                CreatedDate = DateTime.Now,
                GroupId = Code.GenerateProfileGroupId(callingUser.Id, targetUserId),
                IsSeen = false,
                FromUserId = callingUser.Id,
                NotificationTypeId = NotificationType.VideoCall,
                CommentId = Guid.NewGuid().ToString(),
                ToUserId = targetUserId
            };
            Comment commentModel = new Comment()
            {
                Content = msg,
                Created = DateTime.Now,
                CreatedByAdmin = false,
                Creator = callingUser.Id,
                FileMimeType = null,
                FileURL = null,
                Id = Guid.NewGuid().ToString(),
                Parent = null,
                Root = null,
                UpvoteCount = 0,
                UserHasUpvoted = false,
                UserVoted = "",
                CreatedByCurrentUser = false,
                IsNew = false,
                Modified = DateTime.MinValue,
                Pings = null,
                File = null,
                GroupId = Code.GenerateProfileGroupId(callingUser.Id, targetUserId),
                CommentGroupTypeId = CommentGroupType.ProfileChat
            };
            _context.Comment.Add(commentModel);
            _context.Notification.Add(notification);
            _context.SaveChanges();
        }

        public void AnswerCall(bool acceptCall, string targetConnectionId)
        {
            var callingUser = Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
            var targetUser = Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId);

            // This can only happen if the server-side came down and clients were cleared, while the user
            // still held their browser session.
            if (callingUser == null)
            {
                return;
            }

            // Make sure the original caller has not left the page yet
            if (targetUser == null)
            {
                Clients.Caller.SendAsync("callEnded", targetConnectionId,
                    "The other user in your call has left.");

                return;
            }

            // Send a decline message if the callee said no
            if (acceptCall == false)
            {
                Clients.Client(targetConnectionId).SendAsync("callDeclined",
                   $"{callingUser.Username} did not accept your call.");

                return;
            }

            // Make sure there is still an active offer.  If there isn't, then the other use hung up before the Callee answered.
            var offerCount = CallOffers.RemoveAll(c => c.Callee.ConnectionId == callingUser.ConnectionId
                                                  && c.Caller.ConnectionId == targetUser.ConnectionId);
            if (offerCount < 1)
            {
                Clients.Caller.SendAsync("callEnded", targetConnectionId, $"{targetUser.Username} has already hung up.");

                return;
            }

            // And finally... make sure the user hasn't accepted another call already
            if (GetUserCall(targetUser.ConnectionId) != null)
            {
                // And that they aren't already in a call
                Clients.Caller.SendAsync("callDeclined", $"{targetUser.Username} chose to accept someone elses call instead of yours :(");
                return;
            }

            // Remove all the other offers for the call initiator, in case they have multiple calls out
            CallOffers.RemoveAll(c => c.Caller.ConnectionId == targetUser.ConnectionId);

            // Create a new call to match these folks up
            UserCalls.Add(new UserCall
            {
                Users = new List<User> { callingUser, targetUser }
            });

            // Tell the original caller that the call was accepted
            Clients.Client(targetConnectionId).SendAsync("callAccepted", callingUser);

        }

        public void HangUp(string userName = "")
        {
            var callingUser = Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);

            if (callingUser == null)
            {
                return;
            }


            var currentCall = GetUserCall(callingUser.ConnectionId);

            // Send a hang up message to each user in the call, if there is one
            if (currentCall != null)
            {
                foreach (var user in currentCall.Users.Where(u => u.ConnectionId != callingUser.ConnectionId))
                {
                    Clients.Client(user.ConnectionId).SendAsync("callEnded", callingUser.ConnectionId, $"{callingUser.Username} has hung up.");
                }

                // Remove the call from the list if there is only one (or none) person left.  This should
                // always trigger now, but will be useful when we implement conferencing.
                currentCall.Users.RemoveAll(u => u.ConnectionId == callingUser.ConnectionId);
                if (currentCall.Users.Count < 2)
                {
                    UserCalls.Remove(currentCall);
                }
            }
            else if (!string.IsNullOrEmpty(userName))//case: callee not accept the call, and caller cancel the call
            {

                var targetUserDbInfo = _context.Users.SingleOrDefault(u => u.UserName == userName);
                var targetUser = Users.SingleOrDefault(u => u.Id == targetUserDbInfo.Id);

                if (targetUserDbInfo != null && targetUser != null)
                {
                    Clients.Client(targetUser.ConnectionId).SendAsync("callerCanceled", callingUser.Username);
                }
            }



            // Remove all offers initiating from the caller
            CallOffers.RemoveAll(c => c.Caller.ConnectionId == callingUser.ConnectionId);

        }

        // WebRTC Signal Handler
        public void SendSignal(string signal, string targetConnectionId)
        {
            var callingUser = Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
            var targetUser = Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId);

            // Make sure both users are valid
            if (callingUser == null || targetUser == null)
            {
                return;
            }

            // Make sure that the person sending the signal is in a call
            var userCall = GetUserCall(callingUser.ConnectionId);

            // ...and that the target is the one they are in a call with
            if (userCall != null && userCall.Users.Exists(u => u.ConnectionId == targetUser.ConnectionId))
            {
                // These folks are in a call together, let's let em talk WebRTC
                Clients.Client(targetConnectionId).SendAsync("receiveSignal", callingUser, signal);

            }
        }

        #region Private Helpers

        private UserCall GetUserCall(string connectionId)
        {
            var matchingCall =
                UserCalls.SingleOrDefault(uc => uc.Users.SingleOrDefault(u => u.ConnectionId == connectionId) != null);
            return matchingCall;
        }

        #endregion
    }

}
