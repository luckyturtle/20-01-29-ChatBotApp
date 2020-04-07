/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Models;
using Link.Models.GroupModels;
using Link.Models.GroupViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Link.Controllers.Api
{
    [Produces("application/json")]
    [Authorize]

    public class GroupsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int NumberOfLoadingItems = 8;
        public GroupsController(ApplicationDbContext context,
                                  UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("api/[controller]/numberOfGroup")]
        public async Task<int> GetNumberOfGroupAsync()
        {
            return await _context.Group.CountAsync();
        }

        [HttpGet]
        [Route("api/[controller]/numberOfCurrentUserGroup")]
        public async Task<int> GetNumberOfCurrentUserGroupAsync()
        {
            string currentUserId = _userManager.GetUserId(User);

            return await _context.GroupMember.CountAsync(gm => gm.MemberId == currentUserId);
        }


        [HttpGet]
        [Route("api/[controller]/loadCurrentUserGroups/{name}/{index}")]
        public IActionResult LoadCurrentUserGroups(string name, int index)
        {
            if (index >= 0)
            {
                string currentUserId = _userManager.GetUserId(User);

                IQueryable<GroupMember> currentUserGroup = _context.GroupMember.Where(m => m.MemberId == currentUserId);

                if (currentUserGroup != null)
                {
                    List<GroupViewModel> groupViewModelList = new List<GroupViewModel>();

                    IQueryable<string> currentUserGroupId = currentUserGroup.Select(m => m.GroupId);

                    //get group information
                    IQueryable<Group> groupList = _context.Group
                        .Where(g => currentUserGroupId.Contains(g.Id) && (name == "*" ? true : g.Name.Contains(name)))
                        .OrderByDescending(g => g.Created)
                        .Skip(index)
                        .Take(NumberOfLoadingItems);

                    //prepare  group information as GroupViewModel format
                    foreach (Group group in groupList)
                    {
                        GroupMember currentMemberGroupInfo = currentUserGroup.SingleOrDefault(o => o.GroupId == group.Id);

                        groupViewModelList.Add(new GroupViewModel()
                        {
                            Description = group.Description,
                            Id = group.Id,
                            IsAdmin = (group.Creator == currentUserId)
                            || (currentMemberGroupInfo != null ? currentMemberGroupInfo.IsAdmin : false), //user can manage group (i.e. view,edit,delete) if he/she admin in the group.
                            Name = group.Name,
                            NumberOfMember = group.NumberOfMember,
                            NumberOfViewer = group.NumberOfViewer,
                            PhotoIsExist = group.Photo != null,
                            IsCurrentUserFollowGroup = true
                        });
                    }

                    return Json(groupViewModelList);
                }
            }
            return Json(new List<GroupViewModel>());
        }

        [HttpGet]
        [Route("api/[controller]/{name}/{index}")]
        public IActionResult GetGroup(string name, int index)
        {
            if (index >= 0)
            {
                string currentUserId = _userManager.GetUserId(User);

                IQueryable<GroupMember> currentUserGroup = _context.GroupMember.Where(m => m.MemberId == currentUserId);

                if (currentUserGroup != null)
                {
                    List<GroupViewModel> groupViewModelList = new List<GroupViewModel>();

                    IQueryable<string> currentUserGroupId = currentUserGroup.Select(m => m.GroupId);

                    //get the group iformation
                    IQueryable<Group> groupList = _context.Group
                                                        .Where(g => (currentUserGroupId.Contains(g.Id) || g.GroupTypeId == GroupType.Public) && (name == "*" ? true : g.Name.Contains(name)))
                                                        .OrderByDescending(g => g.Created)
                                                        .Skip(index)
                                                        .Take(6);

                    //prepare the group information to be in GroupViewModel format
                    foreach (Group group in groupList)
                    {
                        GroupMember currentMemberGroupInfo = currentUserGroup.SingleOrDefault(o => o.GroupId == group.Id);

                        groupViewModelList.Add(new GroupViewModel()
                        {
                            Description = group.Description,
                            Id = group.Id,
                            IsAdmin = (group.Creator == currentUserId)
                            || (currentMemberGroupInfo != null ? currentMemberGroupInfo.IsAdmin : false), //user can manage group (i.e. view,edit,delete) if he/she admin in the group.
                            Name = group.Name,
                            NumberOfMember = group.NumberOfMember,
                            NumberOfViewer = group.NumberOfViewer,
                            PhotoIsExist = group.Photo != null,

                            IsCurrentUserFollowGroup = currentUserGroup.Any(o => o.MemberId == currentUserId && o.GroupId == group.Id)
                        });
                    }

                    return Json(groupViewModelList);
                }



            }

            return Json(new List<GroupViewModel>());
        }


        [Route("api/Groups/groupChat/{index}")]
        public IActionResult GroupChatSession(int index)
        {
            string currentUserId = _userManager.GetUserId(User);
            if (index >= 0)
            {
                //get the current user group, which he/she was joined
                var currentUserGroup = _context.GroupMember
                                                           .Where(m => m.MemberId == currentUserId)
                                                           .Select(gm => new
                                                           {
                                                               gm.GroupId,
                                                               gm.JoinDate
                                                           })
                                                           .OrderByDescending(g => g.JoinDate)
                                                           .Skip(index)
                                                           .Take(NumberOfLoadingItems).ToList();

                if (currentUserGroup != null)
                {
                    //get the group iformation
                    var groupViewModelList = _context.Group.Where(g => currentUserGroup
                                                          .Select(o => o.GroupId)
                                                           .Contains(g.Id)).Select(group => new GroupViewModel
                                                           {
                                                               Description = group.Description,
                                                               Id = group.Id,
                                                               Name = group.Name,
                                                               PhotoIsExist = group.Photo != null
                                                           });

                    return Json(groupViewModelList);
                }
            }
            return Json(new List<GroupViewModel>());
        }

    }
}