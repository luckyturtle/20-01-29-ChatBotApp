/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Hubs;
using Link.Models;
using Link.Models.CommentModels;
using Link.Models.CommentViewModels;
using Link.Models.GroupModels;
using Link.Models.NotificationModels;
using Link.Models.TransactionModels;
using Link.Models.UserProfileModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link.Controllers.Api
{
    [Produces("application/json")]
    [Authorize]

    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly int NumberOfLoadingItems = 10;
        public CommentsController(ApplicationDbContext context,
                                  UserManager<ApplicationUser> userManager,
                                  IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        // GET: api/Comments/1/2/0
        [HttpGet]
        [Route("api/[controller]/mainComments/{index}")]
        public IEnumerable<CommentViewModel> GetMainComments(int index)
        {
            List<CommentViewModel> commentViewModelList = new List<CommentViewModel>();
            List<string> currentCommentsId = new List<string>();
            var currentUserId = _userManager.GetUserId(User);

            if (index >= 0)
            {
                //get current user profile 
                IQueryable<string> currentUserGroupIds = null;

                currentUserGroupIds =
               _context.GroupMember //get current user groups  
               .Where(m => m.MemberId == currentUserId)
               .Select(g => g.GroupId)
               .Union(_context.Followers //get who current followed 
               .Where(f => f.FollowersId == currentUserId)
               .Select(f => f.UserId));

                var currentCommentList = _context.Comment
                 .Where(c => (currentUserGroupIds.Contains(c.GroupId) || c.GroupId == currentUserId) && c.Parent == null && c.CommentGroupTypeId != CommentGroupType.GroupChat)
                 .OrderByDescending(c => c.Created)
                 .Skip(index)
                 .Take(NumberOfLoadingItems);

                var groupComments = (from comment in currentCommentList
                                     join @group in _context.Group on comment.GroupId equals @group.Id
                                     join user in _context.Users on comment.Creator equals user.Id
                                     select new
                                     {
                                         comment.Content,
                                         comment.Created,
                                         comment.CreatedByAdmin,
                                         CreatedByCurrentUser = comment.Creator,
                                         comment.Creator,
                                         comment.FileMimeType,
                                         comment.FileURL,
                                         FullName = user.UserName,
                                         comment.Id,
                                         comment.IsNew,
                                         comment.Modified,
                                         comment.Parent,
                                         user.GenderId,
                                         comment.UpvoteCount,
                                         comment.UserHasUpvoted,
                                         comment.UserVoted,
                                         comment.Pings,
                                         comment.GroupId,
                                         GroupName = @group.Name,
                                         comment.CommentGroupTypeId,
                                         PostOnURL = string.Format("/Groups/Details/{0}", comment.GroupId)

                                     }).ToList().Union(from comment in currentCommentList
                                                       join @group in _context.Users on comment.GroupId equals @group.Id
                                                       join user in _context.Users on comment.Creator equals user.Id
                                                       select new
                                                       {
                                                           comment.Content,
                                                           comment.Created,
                                                           comment.CreatedByAdmin,
                                                           CreatedByCurrentUser = comment.Creator,
                                                           comment.Creator,
                                                           comment.FileMimeType,
                                                           comment.FileURL,
                                                           FullName = user.UserName,
                                                           comment.Id,
                                                           comment.IsNew,
                                                           comment.Modified,
                                                           comment.Parent,
                                                           user.GenderId,
                                                           comment.UpvoteCount,
                                                           comment.UserHasUpvoted,
                                                           comment.UserVoted,
                                                           comment.Pings,
                                                           comment.GroupId,
                                                           GroupName = user.UserName,
                                                           comment.CommentGroupTypeId,
                                                           PostOnURL = string.Format("/Profile/Index/{0}", user.UserName)
                                                       });

                if (currentCommentList != null)
                {
                    List<string> rootIdList = new List<string>();

                    //preparing post comments 
                    foreach (var comment in groupComments)
                    {
                        CommentViewModel commentViewModel = new CommentViewModel()
                        {
                            Content = comment.Content,
                            Created = comment.Created,
                            CreatedByAdmin = comment.CreatedByAdmin,
                            CreatedByCurrentUser = comment.Creator == currentUserId,
                            Creator = comment.Creator,
                            FileMimeType = comment.FileMimeType,
                            FileURL = comment.FileURL,
                            Fullname = comment.FullName,
                            Id = comment.Id,
                            IsNew = comment.IsNew,
                            Modified = comment.Modified,
                            Parent = comment.Parent,
                            UpvoteCount = comment.UpvoteCount,
                            UserHasUpvoted = !string.IsNullOrEmpty(comment.UserVoted) && comment.UserVoted.Split(',').Any(o => o == currentUserId),
                            UserVoted = comment.UserVoted,
                            Pings = !string.IsNullOrEmpty(comment.Pings) ? comment.Pings.Split(',') : null,
                            GroupId = comment.GroupId,
                            GroupName = comment.CommentGroupTypeId == CommentGroupType.Profile ? comment.Creator == currentUserId ? " your profile " : (comment.GenderId == Gender.Female ? "her profile" : "his profile") : comment.GroupName + " group",
                            PostOnURL = comment.PostOnURL
                        };

                        rootIdList.Add(comment.Id);
                        commentViewModelList.Add(commentViewModel);
                    }

                    currentCommentsId.AddRange(rootIdList);

                    //Get all post child list, where root id=post id
                    // var postChildList = new List<CommentViewModel>();
                    if (rootIdList.Count >= 1)
                    {
                        //add parent comment childs
                        var commentChilds = _context.Comment.Where(c => rootIdList.Contains(c.Root))
                            .Join(_context.Users, c => c.Creator, u => u.Id, (comment, user) =>
                           new
                           {
                               comment.Content,
                               comment.Created,
                               comment.CreatedByAdmin,
                               CreatedByCurrentUser = comment.Creator,
                               comment.Creator,
                               comment.FileMimeType,
                               comment.FileURL,
                               FullName = user.UserName,
                               comment.Id,
                               comment.IsNew,
                               comment.Modified,
                               comment.Parent,
                               comment.UpvoteCount,
                               comment.UserHasUpvoted,
                               comment.UserVoted,
                               comment.Pings,
                               comment.Root
                           }).OrderBy(c => c.Created);

                        foreach (var comment in commentChilds)
                        {
                            //preparing child comments  
                            CommentViewModel commentViewModel = new CommentViewModel()
                            {
                                Content = comment.Content,
                                Created = comment.Created,
                                CreatedByAdmin = comment.CreatedByAdmin,
                                CreatedByCurrentUser = comment.Creator == currentUserId,
                                Creator = comment.Creator,
                                FileMimeType = comment.FileMimeType,
                                FileURL = comment.FileURL,
                                Fullname = comment.FullName,
                                Root = comment.Root,
                                Id = comment.Id,
                                IsNew = comment.IsNew,
                                Modified = comment.Modified,
                                Parent = comment.Parent,
                                UpvoteCount = comment.UpvoteCount,
                                UserHasUpvoted = !string.IsNullOrEmpty(comment.UserVoted) && (comment.UserVoted.Split(',').Any(o => o == currentUserId)),
                                UserVoted = comment.UserVoted,
                                Pings = !string.IsNullOrEmpty(comment.Pings) ? comment.Pings.Split(',') : null
                            };
                            commentViewModelList.Add(commentViewModel);
                            currentCommentsId.Add(comment.Id);
                        }
                    }
                }

                SetCurrentCommentsNotificationsAsSeen(currentUserId, currentCommentsId);

            }
            return commentViewModelList;
        }

        // GET: api/Comments/1/2/0
        [HttpGet]
        [Route("api/[controller]/{id}/{type}/{index}")]
        public IEnumerable<CommentViewModel> GetComments(string id, int type, int index)
        {
            List<CommentViewModel> commentViewModelList = new List<CommentViewModel>();
            List<string> currentCommentsId = new List<string>();
            var currentUserId = _userManager.GetUserId(User);

            bool isCurrentUserAllowToSeeTheSelectedGroup = IsCurrentUserAllowToSeeTheSelectedGroup(type, id, currentUserId);

            if (index >= 0 && isCurrentUserAllowToSeeTheSelectedGroup)
            {
                var currentCommentList = _context.Comment
                    .Where(c => c.GroupId == id && c.CommentGroupTypeId == type && c.Parent == null)
                    .OrderByDescending(c => c.Created)
                    .Skip(index)
                    .Take(NumberOfLoadingItems)
                     .Join(_context.Users, c => c.Creator, u => u.Id, (comment, user) =>
                            new
                            {
                                comment.Content,
                                comment.Created,
                                comment.CreatedByAdmin,
                                CreatedByCurrentUser = comment.Creator,
                                comment.Creator,
                                comment.FileMimeType,
                                comment.FileURL,
                                FullName = user.UserName,
                                comment.Id,
                                comment.IsNew,
                                comment.Modified,
                                comment.Parent,
                                comment.UpvoteCount,
                                comment.UserHasUpvoted,
                                comment.UserVoted,
                                comment.Pings,
                                comment.GroupId
                            });


                if (currentCommentList != null)
                {
                    List<string> rootIdList = new List<string>();
                    //preparing post comments 
                    foreach (var comment in currentCommentList)
                    {
                        CommentViewModel commentViewModel = new CommentViewModel()
                        {
                            Content = comment.Content,
                            Created = comment.Created,
                            CreatedByAdmin = comment.CreatedByAdmin,
                            CreatedByCurrentUser = comment.Creator == currentUserId,
                            Creator = comment.Creator,
                            FileMimeType = comment.FileMimeType,
                            FileURL = comment.FileURL,
                            Fullname = comment.FullName,
                            Id = comment.Id,
                            IsNew = comment.IsNew,
                            Modified = comment.Modified,
                            Parent = comment.Parent,
                            UpvoteCount = comment.UpvoteCount,
                            UserHasUpvoted = !string.IsNullOrEmpty(comment.UserVoted) && (comment.UserVoted.Split(',').Any(o => o == currentUserId)),
                            UserVoted = comment.UserVoted,
                            Pings = !string.IsNullOrEmpty(comment.Pings) ? comment.Pings.Split(',') : null
                        };

                        rootIdList.Add(comment.Id);
                        commentViewModelList.Add(commentViewModel);
                    }

                    currentCommentsId.AddRange(rootIdList);

                    //Get all post child list, where root id=post id
                    // var postChildList = new List<CommentViewModel>();
                    if (rootIdList.Count >= 1)
                    {
                        //add parent comment childs
                        var commentChilds = _context.Comment.Where(c => rootIdList.Contains(c.Root))
                            .Join(_context.Users, c => c.Creator, u => u.Id, (comment, user) =>
                           new
                           {
                               comment.Content,
                               comment.Created,
                               comment.CreatedByAdmin,
                               CreatedByCurrentUser = comment.Creator,
                               comment.Creator,
                               comment.FileMimeType,
                               comment.FileURL,
                               FullName = user.UserName,
                               comment.Id,
                               comment.IsNew,
                               comment.Modified,
                               comment.Parent,
                               comment.UpvoteCount,
                               comment.UserHasUpvoted,
                               comment.UserVoted,
                               comment.Pings,
                               comment.Root
                           }).OrderBy(c => c.Created);

                        foreach (var comment in commentChilds)
                        {
                            //preparing child comments  
                            CommentViewModel commentViewModel = new CommentViewModel()
                            {
                                Content = comment.Content,
                                Created = comment.Created,
                                CreatedByAdmin = comment.CreatedByAdmin,
                                CreatedByCurrentUser = comment.Creator == currentUserId,
                                Creator = comment.Creator,
                                FileMimeType = comment.FileMimeType,
                                FileURL = comment.FileURL,
                                Fullname = comment.FullName,
                                Root = comment.Root,
                                Id = comment.Id,
                                IsNew = comment.IsNew,
                                Modified = comment.Modified,
                                Parent = comment.Parent,
                                UpvoteCount = comment.UpvoteCount,
                                UserHasUpvoted = !string.IsNullOrEmpty(comment.UserVoted) && (comment.UserVoted.Split(',').Any(o => o == currentUserId)),
                                UserVoted = comment.UserVoted,
                                Pings = !string.IsNullOrEmpty(comment.Pings) ? comment.Pings.Split(',') : null
                            };
                            commentViewModelList.Add(commentViewModel);
                            currentCommentsId.Add(comment.Id);
                        }
                    }
                }

                SetCurrentCommentsNotificationsAsSeen(currentUserId, currentCommentsId);

            }
            return commentViewModelList;
        }

        private bool IsCurrentUserAllowToSeeTheSelectedGroup(int? commentType, string group, string currentUserId)
        {
            // we need to check if the current user is allow to see the selected comment.
            // user allow to see the comment if:
            // current comment part of his/her profile
            // current comment part of gorup/profile followed

            var groupSplit = group.Split("__");
            string followedId = null;
            string userId = null;

            if (groupSplit != null && groupSplit.Count() == 2)
            {
                if (groupSplit[0] == currentUserId)
                {
                    followedId = groupSplit[1];
                    userId = groupSplit[0];
                }
                else if (groupSplit[1] == currentUserId)
                {
                    followedId = groupSplit[0];
                    userId = groupSplit[1];
                }

            }

            var status = group == currentUserId
              || _context.GroupMember.Any(m => m.GroupId == group && m.MemberId == currentUserId)
              || (followedId != null && _context.Followers.Any(f => f.FollowersId == currentUserId && f.UserId == followedId))//for chat session comments
              || _context.Followers.Any(f => f.FollowersId == currentUserId && f.UserId == group)
              || (commentType != null && (commentType.Value == CommentGroupType.GroupChat || commentType.Value == CommentGroupType.ProfileChat) && currentUserId == userId);//for unfriend chat session

            return status;
        }

        [HttpGet]
        [Route("api/[controller]/{createBy}/{contain}/{fromDate}/{toDate}/{group}/{index}")]
        public IEnumerable<CommentViewModel> GetSearchResultComments(string createBy, string contain, string fromDate, string toDate, string group, int index)
        {
            List<CommentViewModel> commentViewModelList = new List<CommentViewModel>();
            List<string> currentCommentsId = new List<string>();
            var currentUserId = _userManager.GetUserId(User);

            if (index >= 0)
            {
                var currentUserGroupIds =
                     _context.GroupMember //get current user groups  
                     .Where(m => m.MemberId == currentUserId)
                     .Select(g => g.GroupId)
                     .Union(_context.Followers //get who current followed 
                     .Where(f => f.FollowersId == currentUserId)
                     .Select(f => f.UserId));

                IQueryable<string> userIdList = null;
                if (createBy != "*")
                {
                    userIdList = _context.Users
                                             .Where(u => u.UserName
                                             .Contains(createBy))
                                             .Select(u => u.Id);

                }

                IQueryable<string> groupIdList = null;
                if (group != "*")
                {
                    groupIdList = _context.Group
                        .Where(g => (g.GroupTypeId == GroupType.Public
                        || _context.GroupMember.Where(m => m.MemberId == currentUserId)
                        .Select(gm => gm.GroupId).Contains(g.Id)) && g.Name.Contains(group))
                        .Select(g => g.Id);
                }

                DateTime fDate = DateTime.MinValue;
                DateTime.TryParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fDate);

                DateTime tDate = DateTime.MinValue;
                DateTime.TryParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tDate);

                IQueryable<Comment> currentCommentList = _context.Comment.Where(c => (currentUserGroupIds.Contains(c.GroupId) || c.GroupId == currentUserId) && c.Parent == null && c.CommentGroupTypeId != CommentGroupType.GroupChat);

                if (contain != "*")
                {
                    currentCommentList = currentCommentList.Where(c => c.Content.Contains(contain));
                }
                if (groupIdList != null)
                {
                    currentCommentList = currentCommentList.Where(c => groupIdList.Contains(c.GroupId));
                }
                if (userIdList != null)
                {
                    currentCommentList = currentCommentList.Where(c => userIdList.Contains(c.Creator));
                }
                if (fDate != DateTime.MinValue)
                {
                    currentCommentList = currentCommentList.Where(c => c.Created >= fDate);
                }
                if (tDate != DateTime.MinValue)
                {
                    currentCommentList = currentCommentList.Where(c => c.Created <= tDate);
                }

                var currentCommenstList = currentCommentList
                     .OrderByDescending(c => c.Created)
                     .Skip(index)
                     .Take(NumberOfLoadingItems);


                var groupComments = (from comment in currentCommenstList
                                     join groups in _context.Group on comment.GroupId equals groups.Id
                                     join user in _context.Users on comment.Creator equals user.Id
                                     select new
                                     {
                                         comment.Content,
                                         comment.Created,
                                         comment.CreatedByAdmin,
                                         CreatedByCurrentUser = comment.Creator,
                                         comment.Creator,
                                         comment.FileMimeType,
                                         comment.FileURL,
                                         FullName = user.UserName,
                                         comment.Id,
                                         comment.IsNew,
                                         comment.Modified,
                                         comment.Parent,
                                         user.GenderId,
                                         comment.UpvoteCount,
                                         comment.UserHasUpvoted,
                                         comment.UserVoted,
                                         comment.Pings,
                                         comment.GroupId,
                                         GroupName = groups.Name,
                                         comment.CommentGroupTypeId,
                                         PostOnURL = string.Format("/Groups/Details/{0}", comment.GroupId)

                                     }).ToList().Union(from comment in currentCommenstList
                                                       join groups in _context.Users on comment.GroupId equals groups.Id
                                                       join user in _context.Users on comment.Creator equals user.Id
                                                       select new
                                                       {
                                                           comment.Content,
                                                           comment.Created,
                                                           comment.CreatedByAdmin,
                                                           CreatedByCurrentUser = comment.Creator,
                                                           comment.Creator,
                                                           comment.FileMimeType,
                                                           comment.FileURL,
                                                           FullName = user.UserName,
                                                           comment.Id,
                                                           comment.IsNew,
                                                           comment.Modified,
                                                           comment.Parent,
                                                           user.GenderId,
                                                           comment.UpvoteCount,
                                                           comment.UserHasUpvoted,
                                                           comment.UserVoted,
                                                           comment.Pings,
                                                           comment.GroupId,
                                                           GroupName = user.UserName,
                                                           comment.CommentGroupTypeId,
                                                           PostOnURL = string.Format("/Profile/Index/{0}", user.UserName)
                                                       });



                if (groupComments != null)
                {
                    List<string> rootIdList = new List<string>();

                    //preparing post comments 
                    foreach (var comment in groupComments)
                    {
                        CommentViewModel commentViewModel = new CommentViewModel()
                        {
                            Content = comment.Content,
                            Created = comment.Created,
                            CreatedByAdmin = comment.CreatedByAdmin,
                            CreatedByCurrentUser = comment.Creator == currentUserId,
                            Creator = comment.Creator,
                            FileMimeType = comment.FileMimeType,
                            FileURL = comment.FileURL,
                            Fullname = comment.FullName,
                            Id = comment.Id,
                            IsNew = comment.IsNew,
                            Modified = comment.Modified,
                            Parent = comment.Parent,
                            UpvoteCount = comment.UpvoteCount,
                            UserHasUpvoted = !string.IsNullOrEmpty(comment.UserVoted) && (comment.UserVoted.Split(',').Any(o => o == currentUserId)),
                            UserVoted = comment.UserVoted,
                            Pings = !string.IsNullOrEmpty(comment.Pings) ? comment.Pings.Split(',') : null,
                            GroupId = comment.GroupId,
                            GroupName = comment.CommentGroupTypeId == CommentGroupType.Profile ? comment.Creator == currentUserId ? " your profile " : (comment.GenderId == Gender.Female ? "her profile" : "his profile") : comment.GroupName + " group",
                            PostOnURL = comment.PostOnURL
                        };

                        rootIdList.Add(comment.Id);
                        commentViewModelList.Add(commentViewModel);
                    }

                    currentCommentsId.AddRange(rootIdList);

                    //Get all post child list, where root id=post id
                    // var postChildList = new List<CommentViewModel>();
                    if (rootIdList.Count >= 1)
                    {
                        //add parent comment childs
                        var commentChilds = _context.Comment.Where(c => rootIdList.Contains(c.Root))
                            .Join(_context.Users, c => c.Creator, u => u.Id, (comment, user) =>
                           new
                           {
                               comment.Content,
                               comment.Created,
                               comment.CreatedByAdmin,
                               CreatedByCurrentUser = comment.Creator,
                               comment.Creator,
                               comment.FileMimeType,
                               comment.FileURL,
                               FullName = user.UserName,
                               comment.Id,
                               comment.IsNew,
                               comment.Modified,
                               comment.Parent,
                               comment.UpvoteCount,
                               comment.UserHasUpvoted,
                               comment.UserVoted,
                               comment.Pings,
                               comment.Root
                           }).OrderBy(c => c.Created);

                        foreach (var comment in commentChilds)
                        {
                            //preparing child comments  
                            CommentViewModel commentViewModel = new CommentViewModel()
                            {
                                Content = comment.Content,
                                Created = comment.Created,
                                CreatedByAdmin = comment.CreatedByAdmin,
                                CreatedByCurrentUser = comment.Creator == currentUserId,
                                Creator = comment.Creator,
                                FileMimeType = comment.FileMimeType,
                                FileURL = comment.FileURL,
                                Fullname = comment.FullName,
                                Root = comment.Root,
                                Id = comment.Id,
                                IsNew = comment.IsNew,
                                Modified = comment.Modified,
                                Parent = comment.Parent,
                                UpvoteCount = comment.UpvoteCount,
                                UserHasUpvoted = !string.IsNullOrEmpty(comment.UserVoted) && (comment.UserVoted.Split(',').Any(o => o == currentUserId)),
                                UserVoted = comment.UserVoted,
                                Pings = !string.IsNullOrEmpty(comment.Pings) ? comment.Pings.Split(',') : null
                            };
                            commentViewModelList.Add(commentViewModel);
                            currentCommentsId.Add(comment.Id);
                        }
                    }
                }

                SetCurrentCommentsNotificationsAsSeen(currentUserId, currentCommentsId);

            }
            return commentViewModelList;
        }

        [HttpGet]
        [Route("api/[controller]/hashTagComments/{id}/{index}")]
        public IEnumerable<CommentViewModel> GetHashTagComments(string id, int index)
        {
            var tag = "#" + id;
            var currentUserId = _userManager.GetUserId(User);
            var commentViewModelList = new List<CommentViewModel>();

            if (index >= 0)
            {
                var currentCommentList = _context.Comment.Where(c => c.Content.Contains(tag)).OrderByDescending(c => c.Created).Skip(index).Take(NumberOfLoadingItems);

                var groupComments = (from comment in currentCommentList
                                     join groups in _context.Group on comment.GroupId equals groups.Id
                                     join user in _context.Users on comment.Creator equals user.Id
                                     select new
                                     {
                                         comment.Root,
                                         comment.Content,
                                         comment.Created,
                                         comment.CreatedByAdmin,
                                         CreatedByCurrentUser = comment.Creator,
                                         comment.Creator,
                                         comment.FileMimeType,
                                         comment.FileURL,
                                         FullName = user.UserName,
                                         comment.Id,
                                         comment.IsNew,
                                         comment.Modified,
                                         comment.Parent,
                                         user.GenderId,
                                         comment.UpvoteCount,
                                         comment.UserHasUpvoted,
                                         comment.UserVoted,
                                         comment.Pings,
                                         comment.GroupId,
                                         GroupName = groups.Name,
                                         comment.CommentGroupTypeId,
                                         PostOnURL = string.Format("/Groups/Details/{0}", comment.GroupId)

                                     }).ToList().Union(from comment in currentCommentList
                                                       join groups in _context.Users on comment.GroupId equals groups.Id
                                                       join user in _context.Users on comment.Creator equals user.Id
                                                       select new
                                                       {
                                                           comment.Root,
                                                           comment.Content,
                                                           comment.Created,
                                                           comment.CreatedByAdmin,
                                                           CreatedByCurrentUser = comment.Creator,
                                                           comment.Creator,
                                                           comment.FileMimeType,
                                                           comment.FileURL,
                                                           FullName = user.UserName,
                                                           comment.Id,
                                                           comment.IsNew,
                                                           comment.Modified,
                                                           comment.Parent,
                                                           user.GenderId,
                                                           comment.UpvoteCount,
                                                           comment.UserHasUpvoted,
                                                           comment.UserVoted,
                                                           comment.Pings,
                                                           comment.GroupId,
                                                           GroupName = user.UserName,
                                                           comment.CommentGroupTypeId,
                                                           PostOnURL = string.Format("/Profile/Index/{0}", user.UserName)
                                                       });

                if (groupComments != null)
                {
                    var rootIdList = new List<string>();

                    //preparing post comments 
                    foreach (var comment in groupComments)
                    {
                        if (comment.Root == null)//case when comment is a root
                        {
                            var commentViewModel = new CommentViewModel()
                            {
                                Content = comment.Content,
                                Created = comment.Created,
                                CreatedByAdmin = comment.CreatedByAdmin,
                                CreatedByCurrentUser = comment.Creator == currentUserId,
                                Creator = comment.Creator,
                                FileMimeType = comment.FileMimeType,
                                FileURL = comment.FileURL,
                                Fullname = comment.FileURL,
                                Id = comment.Id,
                                IsNew = comment.IsNew,
                                Modified = comment.Modified,
                                Parent = comment.Parent,
                                UpvoteCount = comment.UpvoteCount,
                                UserHasUpvoted = !string.IsNullOrEmpty(comment.UserVoted) && (comment.UserVoted.Split(',').Any(o => o == currentUserId)),
                                UserVoted = comment.UserVoted,
                                Pings = !string.IsNullOrEmpty(comment.Pings) ? comment.Pings.Split(',') : null,
                                GroupId = comment.GroupId,
                                CommentGroupTypeId = comment.CommentGroupTypeId,
                                Root = comment.Root,
                                GroupName = comment.CommentGroupTypeId == CommentGroupType.Profile ? comment.Creator == currentUserId ? " your profile " : (comment.GenderId == Gender.Female ? "her profile" : "his profile") : comment.GroupName + " group",
                                PostOnURL = comment.PostOnURL
                            };

                            rootIdList.Add(comment.Id);
                            commentViewModelList.Add(commentViewModel);
                        }
                        else // case the comment is a child
                        {
                            var root = _context.Comment.SingleOrDefault(c => c.Id == comment.Root);
                            if (root != null)
                            {
                                var commentViewModel = new CommentViewModel()
                                {
                                    Content = root.Content,
                                    Created = root.Created,
                                    CreatedByAdmin = root.CreatedByAdmin,
                                    CreatedByCurrentUser = root.Creator == currentUserId,
                                    Creator = root.Creator,
                                    FileMimeType = root.FileMimeType,
                                    FileURL = root.FileURL,
                                    Fullname = root.FileURL,
                                    Id = root.Id,
                                    IsNew = root.IsNew,
                                    Modified = root.Modified,
                                    Parent = root.Parent,
                                    UpvoteCount = root.UpvoteCount,
                                    UserHasUpvoted = !string.IsNullOrEmpty(root.UserVoted) && (root.UserVoted.Split(',').Any(o => o == currentUserId)),
                                    UserVoted = root.UserVoted,
                                    Pings = !string.IsNullOrEmpty(root.Pings) ? root.Pings.Split(',') : null,
                                    GroupId = root.GroupId,
                                    CommentGroupTypeId = root.CommentGroupTypeId,
                                    Root = root.Root
                                };

                                rootIdList.Add(root.Id);
                                commentViewModelList.Add(commentViewModel);
                            }
                        }

                    }

                    //Get all post child list, where root id=post id
                    // var postChildList = new List<CommentViewModel>();
                    if (rootIdList.Count >= 1)
                    {
                        //add parent comment childs
                        var commentChilds = _context.Comment.Where(c => rootIdList.Contains(c.Root)).OrderBy(c => c.Created);

                        foreach (var comment in commentChilds)
                        {
                            //preparing child comments  
                            var commentViewModel = new CommentViewModel()
                            {
                                Content = comment.Content,
                                Created = comment.Created,
                                CreatedByAdmin = comment.CreatedByAdmin,
                                CreatedByCurrentUser = comment.Creator == currentUserId,
                                Creator = comment.Creator,
                                FileMimeType = comment.FileMimeType,
                                FileURL = comment.FileURL,
                                Fullname = comment.FileURL,
                                Root = comment.Root,
                                Id = comment.Id,
                                IsNew = comment.IsNew,
                                Modified = comment.Modified,
                                Parent = comment.Parent,
                                UpvoteCount = comment.UpvoteCount,
                                UserHasUpvoted = !string.IsNullOrEmpty(comment.UserVoted) && (comment.UserVoted.Split(',').Any(o => o == currentUserId)),
                                UserVoted = comment.UserVoted,
                                Pings = !string.IsNullOrEmpty(comment.Pings) ? comment.Pings.Split(',') : null,
                                GroupId = comment.GroupId,
                                CommentGroupTypeId = comment.CommentGroupTypeId
                            };
                            commentViewModelList.Add(commentViewModel);
                        }
                    }
                }
            }
            return commentViewModelList;
        }

        // GET: api/Comments/1
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public IEnumerable<CommentViewModel> GetComments(string id)
        {
            string currentUserId = _userManager.GetUserId(User);
            List<CommentViewModel> commentViewModelList = new List<CommentViewModel>();
            List<string> currentCommentsId = new List<string>();


            Comment commentInfo = _context.Comment.SingleOrDefault(c => c.Id == id);

            if (commentInfo == null || !IsCurrentUserAllowToSeeTheSelectedGroup(commentInfo.CommentGroupTypeId, commentInfo.GroupId, currentUserId))
            {
                return commentViewModelList;
            }

            //get the comment root if it is a child
            if (commentInfo.Root != null)
            {
                commentInfo = _context.Comment.SingleOrDefault(c => c.Id == commentInfo.Root);
            }

            ApplicationUser commentCreator = _context.Users.SingleOrDefault(u => u.Id == commentInfo.Creator);
            if (commentCreator != null)
            {
                CommentViewModel commentViewModel = new CommentViewModel()
                {
                    Content = commentInfo.Content,
                    Created = commentInfo.Created,
                    CreatedByAdmin = commentInfo.CreatedByAdmin,
                    CreatedByCurrentUser = commentInfo.Creator == currentUserId,
                    Creator = commentInfo.Creator,
                    FileMimeType = commentInfo.FileMimeType,
                    FileURL = commentInfo.FileURL,
                    Fullname = commentCreator.UserName,
                    Id = commentInfo.Id,
                    IsNew = commentInfo.IsNew,
                    Modified = commentInfo.Modified,
                    Parent = commentInfo.Parent,
                    UpvoteCount = commentInfo.UpvoteCount,
                    UserHasUpvoted = !string.IsNullOrEmpty(commentInfo.UserVoted) && (commentInfo.UserVoted.Split(',').Any(o => o == currentUserId)),
                    UserVoted = commentInfo.UserVoted,
                    Pings = !string.IsNullOrEmpty(commentInfo.Pings) ? commentInfo.Pings.Split(',') : null
                };

                commentViewModelList.Add(commentViewModel);
            }
            currentCommentsId.Add(commentInfo.Id);


            //add parent comment childs
            var commentChilds = _context.Comment.Where(c => c.Root == commentInfo.Id)
                .Join(_context.Users, c => c.Creator, u => u.Id, (comment, user) =>
               new
               {
                   comment.Content,
                   comment.Created,
                   comment.CreatedByAdmin,
                   CreatedByCurrentUser = comment.Creator,
                   comment.Creator,
                   comment.FileMimeType,
                   comment.FileURL,
                   FullName = user.UserName,
                   comment.Id,
                   comment.IsNew,
                   comment.Modified,
                   comment.Parent,
                   comment.UpvoteCount,
                   comment.UserHasUpvoted,
                   comment.UserVoted,
                   comment.Pings,
                   comment.Root
               }).OrderBy(c => c.Created);

            foreach (var comment in commentChilds)
            {
                //preparing child comments  
                CommentViewModel commentViewModel = new CommentViewModel()
                {
                    Content = comment.Content,
                    Created = comment.Created,
                    CreatedByAdmin = comment.CreatedByAdmin,
                    CreatedByCurrentUser = comment.Creator == currentUserId,
                    Creator = comment.Creator,
                    FileMimeType = comment.FileMimeType,
                    FileURL = comment.FileURL,
                    Fullname = comment.FullName,
                    Root = comment.Root,
                    Id = comment.Id,
                    IsNew = comment.IsNew,
                    Modified = comment.Modified,
                    Parent = comment.Parent,
                    UpvoteCount = comment.UpvoteCount,
                    UserHasUpvoted = !string.IsNullOrEmpty(comment.UserVoted) && (comment.UserVoted.Split(',').Any(o => o == currentUserId)),
                    UserVoted = comment.UserVoted,
                    Pings = !string.IsNullOrEmpty(comment.Pings) ? comment.Pings.Split(',') : null
                };
                currentCommentsId.Add(comment.Id);

                commentViewModelList.Add(commentViewModel);
            }

            SetCurrentCommentsNotificationsAsSeen(currentUserId, currentCommentsId);

            return commentViewModelList;
        }

        // PUT: api/Comments/1
        [HttpPut]
        [Route("api/[controller]/{id}")]
        public async Task<IActionResult> PutComment(string id, CommentViewModel comment)
        {
            string currentUserId = _userManager.GetUserId(User);

            comment.Modified = DateTime.Now;
            ModelState.Remove("Modified");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Comment commentModel = await _context.Comment.SingleOrDefaultAsync(c => c.Id == comment.Id);
            if (id != comment.Id || commentModel == null)
            {
                return BadRequest();
            }

            commentModel.Content = comment.Content;
            commentModel.Modified = DateTime.Now;
            commentModel.Pings = comment.Pings != null ? string.Join(",", comment.Pings) : null;

            _context.Entry(commentModel).State = EntityState.Modified;
            CreateTransaction(comment, currentUserId, CommentTransactionType.Edit);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(comment);
        }

        [HttpGet]
        [Route("api/[controller]/Download/{id}")]
        public async Task<IActionResult> Download(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Comment comment = await _context.Comment.SingleOrDefaultAsync(m => m.Id == id);
            return File(comment.File, comment.FileMimeType, comment.FileURL);
        }

        [HttpPost]
        [Route("api/[controller]/Upload")]
        public async Task<IActionResult> UploadAttachment(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Content("file not selected");
            }
            //update file name
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\comments-files",
                        file.FileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(file.FileName);
        }

        // POST: api/Comments
        [HttpPost]
        [Route("api/[controller]/")]
        public async Task<IActionResult> PostComment(CommentViewModel comment)
        {
            byte[] file = GetCommentAttachedFile(comment);

            string currentUserId = _userManager.GetUserId(User);

            comment.Id = Guid.NewGuid().ToString();
            comment.CreatedByCurrentUser = false;
            comment.Creator = currentUserId;
            comment.IsNew = true;
            comment.CreatedByCurrentUser = true;

            #region Get the current comment root Id
            Comment parent = null;
            if (!string.IsNullOrEmpty(comment.Parent))
            {
                parent = _context.Comment.SingleOrDefault(o => o.Id == comment.Parent);
                if (parent != null)
                {
                    if (string.IsNullOrEmpty(parent.Parent))
                    {
                        comment.Root = parent.Id;
                    }
                    else
                    {
                        comment.Root = parent.Root;
                    }
                }
            }
            #endregion

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Comment commentModel = new Comment()
            {
                Content = comment.Content ?? "",
                Created = DateTime.Now,
                CreatedByAdmin = false,
                Creator = currentUserId,
                FileMimeType = comment.FileMimeType,
                FileURL = comment.FileURL,
                Id = comment.Id,
                Parent = comment.Parent,
                Root = comment.Root,
                UpvoteCount = 0,
                UserHasUpvoted = false,
                UserVoted = "",
                CreatedByCurrentUser = false,
                IsNew = false,
                Modified = DateTime.MinValue,
                Pings = comment.Pings != null ? string.Join(",", comment.Pings) : null,
                File = file,
                GroupId = parent != null ? parent.GroupId : comment.GroupId,
                CommentGroupTypeId = parent != null ? parent.CommentGroupTypeId : comment.CommentGroupTypeId
            };

            comment.GroupId = commentModel.GroupId;
            comment.CommentGroupTypeId = commentModel.CommentGroupTypeId;

            _context.Comment.Add(commentModel);

            CreateTransaction(comment, currentUserId, CommentTransactionType.Add);

            #region prepare all Notifications types
            await PrepareCommentNotifications(comment, currentUserId);
            #endregion

            await _context.SaveChangesAsync();

            return Json(comment);
        }

        private static byte[] GetCommentAttachedFile(CommentViewModel comment)
        {
            byte[] file = null;
            if (!string.IsNullOrEmpty(comment.FileURL))
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\comments-files",
                        comment.FileURL);

                using (FileStream fsSource = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    file = new byte[fsSource.Length];
                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        int n = fsSource.Read(file, numBytesRead, numBytesToRead);
                        if (n == 0)
                        {
                            break;
                        }

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                }
                System.IO.File.Delete(filePath);
            }

            return file;
        }

        // POST: api/Comments/id/upvotes
        [HttpPut]
        [Route("api/[controller]/{id}/Upvotes/{value}")]
        public async Task<IActionResult> PutUpvotes(string id, int value)
        {
            string currentUserId = _userManager.GetUserId(User);

            int voteValue = (value == 1) ? 1 : -1;

            Comment comment = _context.Comment.SingleOrDefault(o => o.Id == id);
            if (comment == null)
            {
                return BadRequest();
            }

            bool isCurrentUserUpvotedBefor = comment.UserVoted != null && comment.UserVoted.Split(',').Any(o => o == currentUserId);

            if (isCurrentUserUpvotedBefor)
            {
                voteValue = -1; //check if the current user upvoted Before, then subtract one
            }
            else if (currentUserId != comment.Creator)
            {
                await PrepareCommentVotingNotifications(comment, currentUserId); //send notification only when user like the comment
            }

            _context.Entry(comment).Entity.UpvoteCount += voteValue;

            #region UpdateVotedUser
            //Check if user  voted or not. 

            if (voteValue == 1)
            {
                CommentUserVote commentUserVote = new CommentUserVote()
                {
                    CommentId = id,
                    UserId = currentUserId,
                    VoteTypeId = VoteType.Like
                };
                _context.CommentUserVote.Add(commentUserVote);

                _context.Entry(comment).Entity.UserVoted += currentUserId + ",";
            }
            else
            {
                //remove user from the  CommentUserVote table
                CommentUserVote commentUserVote = _context.CommentUserVote.Single(c => c.UserId == currentUserId && c.CommentId == id);
                _context.CommentUserVote.Remove(commentUserVote);

                //remove user from the comment UserVoted field
                StringBuilder userVotedUpdated = new StringBuilder();
                string[] votedUserArray = _context.Entry(comment).Entity.UserVoted.Split(',');
                foreach (string userId in votedUserArray)
                {
                    if (userId != currentUserId)
                    {
                        if (userVotedUpdated.Length == 0)
                        {
                            userVotedUpdated.Append(userId);
                        }
                        else
                        {
                            userVotedUpdated.Append(",").Append(userId);
                        }
                    }
                }
                _context.Entry(comment).Entity.UserVoted = userVotedUpdated.ToString();
            }
            #endregion

            CreateTransaction(comment, currentUserId, CommentTransactionType.Vote);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete]
        [Route("api/[controller]/{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            string currentUserId = _userManager.GetUserId(User);


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Comment comment = await _context.Comment.SingleOrDefaultAsync(m => m.Id == id);
            var notification = _context.Notification.Where(m => m.CommentId == id);//delete commment notifications

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comment.Remove(comment);
            if (notification != null)
            {
                _context.Notification.RemoveRange(notification);
            }

            CreateTransaction(comment, currentUserId, CommentTransactionType.Delete);


            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        private bool CommentExists(string id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }

        #region Prepare Comments Notification
        private async Task PrepareGroupChatNotifications(CommentViewModel comment, string currentUserId)
        {
            //get all current group cliend connection id, and send the message to them.
            var groupsMember = _context.GroupMember.Where(g => g.GroupId == comment.GroupId && g.MemberId != currentUserId)
                 .Join(_context.Group, gm => gm.GroupId, g => g.Id, (gm, g) => new
                 {
                     gm.GroupId,
                     gm.MemberId,
                     GroupName = g.Name
                 });

            // get all the connected participant notification number.
            var connectedMemberNotificationNum = _context.Connections.Where(o => o.UserId != currentUserId)
                                                .GroupJoin(_context.Notification.Where(o => o.GroupId == comment.GroupId && o.IsSeen == false),
                                                          c => c.UserId, n => n.ToUserId, (connection, notifications) =>
                                                         new
                                                         {
                                                             connection.UserId,
                                                             connection.ConnectionID,
                                                             NotificationNum = notifications.Count() + 1//one here is the current added comment
                                                         });

            //prepare the current post info
            CommentViewModel currentComment = new CommentViewModel()
            {
                Content = comment.Content,
                Created = comment.Created,
                CreatedByAdmin = false,
                CreatedByCurrentUser = false,
                Creator = comment.Creator,
                FileMimeType = comment.FileMimeType,
                FileURL = comment.FileURL,
                Fullname = comment.Fullname,
                Id = comment.Id,
                IsNew = true,
                Modified = comment.Modified,
                Parent = comment.Parent,
                UpvoteCount = comment.UpvoteCount,
                UserHasUpvoted = false,
                Pings = comment.Pings,
                CommentGroupTypeId = comment.CommentGroupTypeId,
                FileName = comment.FileName,
                GroupId = comment.GroupId,
                ProfilePictureUrl = comment.ProfilePictureUrl,
                Root = comment.Root,
                UserVoted = comment.UserVoted,
                CreatorName = User.Identity.Name,
                GroupName = GetGommentGroupName(comment)
            };


            Notification notification = null;

            foreach (var member in groupsMember)
            {
                //add notifications to other member
                if (member.MemberId != null)
                {
                    notification = new Notification
                    {
                        Content = comment.Content,
                        CreatedDate = DateTime.Now,
                        GroupId = comment.GroupId,
                        IsSeen = false,
                        FromUserId = currentUserId,
                        NotificationTypeId = NotificationType.NewChatMessage,
                        Title = string.Format(" sent an message {0}", string.IsNullOrEmpty(member.GroupName) ? "" : string.Format(" in {0} chat session ", member.GroupName)), //need to improve in next version
                        CommentId = comment.Id,
                        ToUserId = member.MemberId
                    };

                    if (!string.IsNullOrEmpty(comment.FileURL))
                    {
                        notification.Title = string.Format(" sent an attachment {0}", string.IsNullOrEmpty(member.GroupName) ? "" : string.Format(" in {0} chat session  ", member.GroupName)); //need to improve in next version
                    }
                    _context.Notification.Add(notification);
                }
            }

            //send notifications to current connected comment group member
            foreach (var conn in connectedMemberNotificationNum)
            {
                if (conn.ConnectionID != null)
                {
                    await _hubContext.Clients.Client(conn.ConnectionID).SendAsync("RefreshChatNotificationNum", conn.NotificationNum, currentComment);
                    //  await _hubContext.Clients.Client(conn.ConnectionID).SendAsync("ReceiveMessage", currentComment, conn.NotificationNum);
                }
            }
        }

        private async Task PrepareProfileChatNotifications(CommentViewModel comment, string currentUserId)
        {
            var friendId = comment.GroupId.Split("__").SingleOrDefault(f => f != currentUserId);//select the other user.__ is the friends id seperator

            var friendConnection = _context.Connections.SingleOrDefault(o => o.UserId == friendId);

            var friendNotificationNum = _context.Notification.Count(n => n.ToUserId == friendId && n.IsSeen == false);


            //prepare the current post info
            CommentViewModel currentComment = new CommentViewModel()
            {
                Content = comment.Content,
                Created = comment.Created,
                CreatedByAdmin = false,
                CreatedByCurrentUser = false,
                Creator = comment.Creator,
                FileMimeType = comment.FileMimeType,
                FileURL = comment.FileURL,
                Fullname = comment.Fullname,
                Id = comment.Id,
                IsNew = true,
                Modified = comment.Modified,
                Parent = comment.Parent,
                UpvoteCount = comment.UpvoteCount,
                UserHasUpvoted = false,
                Pings = comment.Pings,
                CommentGroupTypeId = comment.CommentGroupTypeId,
                FileName = comment.FileName,
                GroupId = comment.GroupId,
                ProfilePictureUrl = comment.ProfilePictureUrl,
                Root = comment.Root,
                UserVoted = comment.UserVoted,
                CreatorName = User.Identity.Name,
                GroupName = GetGommentGroupName(comment)
            };


            Notification notification = null;


            //add notifications to other member
            if (friendId != null)
            {
                notification = new Notification
                {
                    Content = comment.Content,
                    CreatedDate = DateTime.Now,
                    GroupId = comment.GroupId,
                    IsSeen = false,
                    FromUserId = currentUserId,
                    NotificationTypeId = NotificationType.NewChatMessage,
                    Title = string.Format(" sent an message to you"),
                    CommentId = comment.Id,
                    ToUserId = friendId
                };

                if (!string.IsNullOrEmpty(comment.FileURL))
                {
                    notification.Title = string.Format(" sent an attachment to you ");
                }
                _context.Notification.Add(notification);
            }


            //send notifications to current connected comment group member

            if (friendConnection != null)
            {
                await _hubContext.Clients.Client(friendConnection.ConnectionID).SendAsync("RefreshChatNotificationNum", friendNotificationNum, currentComment);
                // await _hubContext.Clients.Client(friendConnection.ConnectionID).SendAsync("ReceiveMessage", currentComment, friendNotificationNum);
            }
        }

        private string GetGommentGroupName(CommentViewModel comment)
        {
            var group = _context.Group.SingleOrDefault(o => o.Id == comment.GroupId);
            var groupName = "";
            if (group != null)
            {
                groupName = group.Name;
            }

            return groupName;
        }

        private async Task PrepareGroupMainCommentsNotifications(CommentViewModel comment, string currentUserId)
        {
            //get all current group cliend connection id, and send the message to them.
            var groupsMember = _context.GroupMember.Where(g => g.GroupId == comment.GroupId && g.MemberId != currentUserId)
                 .Join(_context.Group, gm => gm.GroupId, g => g.Id, (gm, g) => new
                 {
                     gm.GroupId,
                     gm.MemberId,
                     GroupName = g.Name
                 });

            // get all the connected participant notification number.
            var connectedMemberNotificationNum = _context.Connections.Where(o => o.UserId != currentUserId)
                                                .GroupJoin(_context.Notification.Where(o => o.GroupId == comment.GroupId && o.IsSeen == false),
                                                          c => c.UserId, n => n.ToUserId, (connection, notifications) =>
                                                         new
                                                         {
                                                             connection.UserId,
                                                             connection.ConnectionID,
                                                             NotificationNum = notifications.Count() + 1//one here is the current added comment
                                                         });


            Notification notification = null;

            foreach (var member in groupsMember)
            {
                //add notifications to other member
                if (member.MemberId != null)
                {
                    notification = new Notification
                    {
                        Content = comment.Content,
                        CreatedDate = DateTime.Now,
                        GroupId = comment.GroupId,
                        IsSeen = false,
                        FromUserId = currentUserId,
                        CommentId = comment.Id,
                        ToUserId = member.MemberId,
                        NotificationTypeId = NotificationType.AddNewComment
                    };

                    if (!string.IsNullOrEmpty(comment.FileURL))
                    {
                        notification.Title = string.Format(" add an attachment {0}", string.IsNullOrEmpty(member.GroupName) ? "" : string.Format(" in {0} ", member.GroupName));
                    }
                    else
                    {
                        notification.Title = string.Format(" add an comment {0}", string.IsNullOrEmpty(member.GroupName) ? "" : string.Format(" in {0} ", member.GroupName));
                    }

                    _context.Notification.Add(notification);

                }

            }

            CommentViewModel currentComment = new CommentViewModel()
            {
                Content = comment.Content,
                Created = comment.Created,
                CreatedByAdmin = false,
                CreatedByCurrentUser = false,
                Creator = comment.Creator,
                FileMimeType = comment.FileMimeType,
                FileURL = comment.FileURL,
                Fullname = comment.Fullname,
                Id = comment.Id,
                IsNew = true,
                Modified = comment.Modified,
                Parent = comment.Parent,
                UpvoteCount = comment.UpvoteCount,
                UserHasUpvoted = false,
                Pings = comment.Pings,
                CommentGroupTypeId = comment.CommentGroupTypeId,
                FileName = comment.FileName,
                GroupId = comment.GroupId,
                ProfilePictureUrl = comment.ProfilePictureUrl,
                Root = comment.Root,
                UserVoted = comment.UserVoted,
                CreatorName = User.Identity.Name,
                GroupName = GetGommentGroupName(comment)
            };

            foreach (var conn in connectedMemberNotificationNum)
            {
                if (conn.ConnectionID != null)
                {
                    await _hubContext.Clients.Client(conn.ConnectionID).SendAsync("RefreshNotificationNum", conn.NotificationNum, currentComment);
                }
            }
        }

        private async Task PrepareProfileMainCommentsNotifications(CommentViewModel comment, string currentUserId)
        {
            var profileFollowers = _context.Followers.Where(p => p.UserId == comment.GroupId);

            // get all the connected follower notification number.
            var connectedMemberNotificationNum = _context.Connections.Where(c => c.UserId != currentUserId)
                                                         .Join(profileFollowers, c => c.UserId, f => f.FollowersId, (connection, follower) =>
                                                                new
                                                                {
                                                                    connection.ConnectionID,
                                                                    UserId = follower.FollowersId
                                                                })
                                                 .GroupJoin(_context.Notification.Where(o => o.GroupId == comment.GroupId && o.IsSeen == false),
                                                        c => c.UserId, n => n.ToUserId, (connection, notifications) =>
                                                       new
                                                       {
                                                           connection.UserId,
                                                           connection.ConnectionID,
                                                           NotificationNum = notifications.Count() + 1//one here is the current added comment
                                                       });

            Notification notification = null;

            foreach (var follower in profileFollowers)
            {
                //add notifications to other member
                if (follower.FollowersId != null)
                {
                    notification = new Notification
                    {
                        Content = comment.Content,
                        CreatedDate = DateTime.Now,
                        GroupId = comment.GroupId,
                        IsSeen = false,
                        FromUserId = currentUserId
                    };

                    notification.NotificationTypeId = NotificationType.AddNewComment;

                    if (!string.IsNullOrEmpty(comment.FileURL))
                    {
                        notification.Title = string.Format(" add a new attachment ");
                    }
                    else
                    {
                        notification.Title = string.Format(" add a new comment ");
                    }

                    notification.CommentId = comment.Id;
                    notification.ToUserId = follower.FollowersId;

                    _context.Notification.Add(notification);
                }

            }


            CommentViewModel currentComment = new CommentViewModel()
            {
                Content = comment.Content,
                Created = comment.Created,
                CreatedByAdmin = false,
                CreatedByCurrentUser = false,
                Creator = comment.Creator,
                FileMimeType = comment.FileMimeType,
                FileURL = comment.FileURL,
                Fullname = comment.Fullname,
                Id = comment.Id,
                IsNew = true,
                Modified = comment.Modified,
                Parent = comment.Parent,
                UpvoteCount = comment.UpvoteCount,
                UserHasUpvoted = false,
                Pings = comment.Pings,
                CommentGroupTypeId = comment.CommentGroupTypeId,
                FileName = comment.FileName,
                GroupId = comment.GroupId,
                ProfilePictureUrl = comment.ProfilePictureUrl,
                Root = comment.Root,
                UserVoted = comment.UserVoted,
                CreatorName = User.Identity.Name,
                GroupName = GetGommentGroupName(comment)
            };

            foreach (var conn in connectedMemberNotificationNum)
            {
                if (conn.ConnectionID != null)
                {
                    await _hubContext.Clients.Client(conn.ConnectionID).SendAsync("RefreshNotificationNum", conn.NotificationNum, currentComment);
                }
            }
        }

        private async Task PrepareReplyNotifications(CommentViewModel comment, string currentUserId)
        {
            //// get the current comment participants (i.e. user add reply on the comment , the post creator)
            var commentTransaction = _context.CommentTransaction
                                             .Where(ct => (ct.CommentRoot == comment.Root || ct.CommentId == comment.Root) &&
                                             ct.CommentTransactionTypeId == CommentTransactionType.Add && ct.UserId != currentUserId);

            var commentParticipants = (from transaction in commentTransaction
                                       join @group in _context.Group on transaction.GroupId equals @group.Id
                                       select new
                                       {
                                           transaction.UserId,
                                           GroupName = @group.Name,
                                           transaction.GroupId,
                                           Type = CommentGroupType.Group
                                       }).ToList().Union(from transaction in commentTransaction
                                                         join @user in _context.Users on transaction.GroupId equals user.Id
                                                         select new
                                                         {
                                                             transaction.UserId,
                                                             GroupName = @user.UserName,
                                                             transaction.GroupId,
                                                             Type = CommentGroupType.Profile
                                                         }).ToList();

            // get all the connected participant notification number.
            var connectedParticipantsNotificationNum = _context.Connections.Where(c => c.UserId != currentUserId)
                                                                .Join(commentParticipants//.Where(p => p.Type == CommentGroupType.Profile),
                                                                 , con => con.UserId, cp => cp.UserId, (connection, participant) =>
                                                                      new
                                                                      {
                                                                          connection.UserId,
                                                                          connection.ConnectionID,
                                                                      })
                                                               .GroupJoin(_context.Notification.Where(o => o.IsSeen == false && o.ToUserId != currentUserId),
                                                                 c => c.UserId, n => n.ToUserId, (connection, notifications) =>
                                                                  new
                                                                  {
                                                                      connection.UserId,
                                                                      connection.ConnectionID,
                                                                      NotificationNum = notifications.Count() + 1 //one here is the current added comment
                                                                  });

            Notification notification = null;

            foreach (var participant in commentParticipants)
            {
                //add notifications to other member
                if (participant.UserId != null)
                {
                    notification = new Notification
                    {
                        Content = comment.Content,
                        CreatedDate = DateTime.Now,
                        GroupId = comment.GroupId,
                        IsSeen = false,
                        FromUserId = currentUserId,
                        NotificationTypeId = NotificationType.AddReplyOnComment,

                        CommentId = comment.Id,
                        ToUserId = participant.UserId
                    };

                    //prepare title
                    //current user profile case
                    if (participant.Type == CommentGroupType.Profile && participant.GroupId == participant.UserId)
                    {
                        notification.Title = "  add an reply on your profile ";
                    }
                    //profile case
                    else if (participant.Type == CommentGroupType.Profile)
                    {
                        notification.Title = string.Format("  add an reply on {0} profile ", participant.GroupName);
                    }
                    //group case
                    else if (participant.Type == CommentGroupType.Group)
                    {
                        notification.Title = string.Format("  add an reply on {0} group ", participant.GroupName);

                    }
                    else
                    {
                        notification.Title = "  add an reply ";
                    }

                    _context.Notification.Add(notification);

                }
            }

            //send notifications to current connected comment particiapant
            foreach (var conn in connectedParticipantsNotificationNum)
            {
                if (conn.ConnectionID != null)
                {
                    await _hubContext.Clients.Client(conn.ConnectionID).SendAsync("RefreshNotificationNum", conn.NotificationNum);
                }
            }
        }

        private async Task PrepareCommentVotingNotifications(Comment comment, string currentUserId)
        {

            var commentCreatorConnection = _context.Connections.SingleOrDefault(o => o.UserId == comment.Creator);
            var commentCreatorNotificationNum = _context.Notification.Count(o => o.ToUserId == comment.Creator && o.IsSeen == false) + 1;

            Notification notification = new Notification
            {
                Content = comment.Content,
                CreatedDate = DateTime.Now,
                GroupId = comment.GroupId,
                IsSeen = false,
                FromUserId = currentUserId,
                NotificationTypeId = NotificationType.AddReplyOnComment,
                Title = string.Format(" like your comment "),
                CommentId = comment.Id,
                ToUserId = comment.Creator
            };

            _context.Notification.Add(notification);

            if (commentCreatorConnection != null)
            {
                await _hubContext.Clients.Client(commentCreatorConnection.ConnectionID).SendAsync("RefreshNotificationNum", commentCreatorNotificationNum);
            }
        }

        private async Task PrepareCommentNotifications(CommentViewModel comment, string currentUserId)
        {
            //Group chat case
            if (comment.CommentGroupTypeId == CommentGroupType.GroupChat)
            {
                await PrepareGroupChatNotifications(comment, currentUserId);
            }

            //Profile chat case
            if (comment.CommentGroupTypeId == CommentGroupType.ProfileChat)
            {
                await PrepareProfileChatNotifications(comment, currentUserId);
            }
            //post on profile case
            else if (comment.CommentGroupTypeId == CommentGroupType.Group && comment.Root == null)
            {
                await PrepareGroupMainCommentsNotifications(comment, currentUserId);

            }
            //profile page case: send notification for all profile follower
            else if (comment.CommentGroupTypeId == CommentGroupType.Profile && comment.Root == null)
            {
                await PrepareProfileMainCommentsNotifications(comment, currentUserId);

            }
            //reply on comment case: send notification for all user who was comment on the post
            else if (comment.Root != null)
            {
                await PrepareReplyNotifications(comment, currentUserId);
            }
        }

        private void CreateTransaction(CommentViewModel comment, string currentUserId, int type)
        {
            _context.CommentTransaction.Add(
             new CommentTransaction
             {
                 CommentId = comment.Id,
                 CommentRoot = comment.Root,
                 CommentTransactionTypeId = type,
                 TimeStamp = DateTime.Now,
                 UserId = currentUserId,
                 Data = JsonConvert.SerializeObject(comment),
                 GroupId = comment.GroupId
             });
        }

        private void CreateTransaction(Comment comment, string currentUserId, int type)
        {
            _context.CommentTransaction.Add(
             new CommentTransaction
             {
                 CommentId = comment.Id,
                 CommentRoot = comment.Root,
                 CommentTransactionTypeId = type,
                 TimeStamp = DateTime.Now,
                 UserId = currentUserId,
                 Data = JsonConvert.SerializeObject(comment),
                 GroupId = comment.GroupId
             });
        }

        private void SetCurrentCommentsNotificationsAsSeen(string currentUserId, List<string> currentCommentsId)
        {
            //set the comment notification as seen by the current user.
            var currentUserNotificationList = _context.Notification.Where(m => m.ToUserId == currentUserId && currentCommentsId.Contains(m.CommentId) && m.IsSeen == false);
            if (currentUserNotificationList != null && currentUserNotificationList.Count() >= 1)
            {
                foreach (var notification in currentUserNotificationList)
                {
                    notification.IsSeen = true;
                }
                _context.SaveChanges();

                //after set the notification as seen, sent the un seen notifications number to current user.
                var currentUserConnectionId = _context.Connections.SingleOrDefault(o => o.UserId == currentUserId);
                var numberOfCurrentUserUnSeenNofification = _context.Notification.Count(m => m.ToUserId == currentUserId && m.NotificationTypeId != NotificationType.NewChatMessage && m.IsSeen == false);
                if (currentUserConnectionId != null)
                {
                    _hubContext.Clients.Client(currentUserConnectionId.ConnectionID).SendAsync("RefreshNotificationNum", numberOfCurrentUserUnSeenNofification);
                }
            }
        }
        #endregion
    }
}