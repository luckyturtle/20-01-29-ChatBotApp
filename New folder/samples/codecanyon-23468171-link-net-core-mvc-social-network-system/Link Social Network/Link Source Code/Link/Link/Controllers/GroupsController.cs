/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Extensions;
using Link.Models;
using Link.Models.EmailLogsModels;
using Link.Models.GroupModels;
using Link.Models.GroupViewModels;
using Link.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
namespace Link.Controllers
{
    [Authorize]

    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailLogs _emailLogs;
        private readonly IEmailSender _emailSender;
        public GroupsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailLogs emailLogs)
        {
            _context = context;
            _userManager = userManager;
            _emailLogs = emailLogs;
            _emailSender = emailSender;
        }


        [HttpPost]
        public async Task<IActionResult> UploadGroupPhoto(string id, IFormFile file)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return Content("file not selected");
            }

            Group group = await _context.Group.SingleOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();

                group.Photo = fileBytes;
            }

            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Group> result = _context.Update(group);

            return Ok();
        }

        public IActionResult ViewImage(string id)
        {
            Group group = _context.Group.SingleOrDefault(m => m.Id == id);
            return File(group.Photo, MediaTypeNames.Application.Octet, group.Name);
        }


        public IActionResult Index()
        {
            string currentUserId = _userManager.GetUserId(User);
            ViewData["UserId"] = currentUserId;
            return View();
        }

        // GET: Groups/
        public IActionResult Groups()
        {
            string currentUserId = _userManager.GetUserId(User);
            ViewData["UserId"] = currentUserId;
            return View();
        }

        /// <summary>
        /// Get the current user top groups
        /// </summary>
        /// <returns></returns>
        public IActionResult TopGroups()
        {
            string currentUserId = _userManager.GetUserId(User);


            //get the current user group, which he/she was joined
            IQueryable<GroupMember> currentUserGroup = _context.GroupMember.Where(m => m.MemberId == currentUserId);

            if (currentUserGroup != null)
            {
                List<GroupViewModel> groupViewModelList = new List<GroupViewModel>();

                IQueryable<string> currentUserGroupId = currentUserGroup.Select(m => m.GroupId);

                //get the group iformation
                IQueryable<Group> groupList = _context.Group.Where(g => currentUserGroupId.Contains(g.Id)).Include(g => g.GroupType).OrderByDescending(g => g.Created).Take(5);

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
                        IsCurrentUserFollowGroup = true
                    });
                }

                return Json(groupViewModelList);
            }

            return Json(new List<GroupViewModel>());
        }

        public IActionResult LoadMoreGroupFollower(string id, int take)
        {
            if (take >= 0)
            {
                GroupMemberViewModel groupMemberViewModel = new GroupMemberViewModel();
                var currentUserGroup = _context.GroupMember.Join(_context.Users, m => m.MemberId, u => u.Id,
                    (member, user) => new
                    {
                        member.MemberId,
                        MemberName = user.UserName,
                        member.IsAdmin,
                        member.JoinDate,
                        member.GroupId,
                        avatarIsExist = user.Avatar != null
                    }

                ).Where(m => m.GroupId == id).OrderByDescending(g => g.JoinDate).Skip(take).Take(6);


                return Json(currentUserGroup);


            }

            return Json(new List<GroupMember>());
        }


        // GET: Groups/Details/5 
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string currentUserId = _userManager.GetUserId(User);
            ViewData["UserId"] = currentUserId;
            Group @group = await _context.Group
                .Include(g => g.GroupType)
                .SingleOrDefaultAsync(m => m.Id == id);

            bool isCurrentUserFollowGroup = _context.GroupMember.Any(g => g.GroupId == group.Id && g.MemberId == currentUserId);
            bool isCurrentUserAdmin = _context.GroupMember.Any(g => g.GroupId == group.Id && g.MemberId == currentUserId && g.IsAdmin);


            if (@group == null)
            {
                return NotFound();
            }

            //ensure that current user allow to see the current selected group.
            //In order to allow user see the group,user should be follower, or admin, or the group is puplic.
            if (!(isCurrentUserFollowGroup || isCurrentUserAdmin) && @group.GroupTypeId == GroupType.Private)
            {
                return NotFound();
            }

            GroupViewModel GroupViewModel = new GroupViewModel
            {
                Id = group.Id,
                Created = group.Created,
                Description = group.Description,
                NumberOfMember = group.NumberOfMember,
                NumberOfViewer = group.NumberOfViewer,
                GroupTypeId = group.GroupTypeId,
                Name = group.Name,
                IsAdmin = isCurrentUserAdmin,
                IsCurrentUserFollowGroup = isCurrentUserFollowGroup,
                PhotoIsExist = group.Photo != null
            };
            return View(GroupViewModel);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            ViewData["GroupTypeId"] = new SelectList(_context.GroupType, "Id", "Id");
            string currentUserId = _userManager.GetUserId(User);
            ViewData["UserId"] = currentUserId;
            return View();
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,GroupTypeId,Description,Photo")] GroupViewModel @group, IFormFile Photo)
        {
            string currentUserId = _userManager.GetUserId(User);
            ViewData["UserId"] = currentUserId;
            if (ModelState.IsValid)
            {

                string groupId = Guid.NewGuid().ToString();
                Group newGroup = new Group
                {
                    Id = groupId,
                    Created = DateTime.Now,
                    Description = @group.Description,
                    Creator = currentUserId,
                    GroupTypeId = @group.GroupTypeId,
                    Name = @group.Name,
                    Photo = @group.PhotoStream
                };

                if (Photo != null && Photo.Length != 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Photo.CopyTo(ms);
                        byte[] fileBytes = ms.ToArray();
                        newGroup.Photo = fileBytes;
                    }
                }

                _context.Add(newGroup);

                _context.GroupMember.Add(new GroupMember()
                {
                    GroupId = groupId,
                    IsAdmin = true,
                    JoinDate = DateTime.Now,
                    MemberId = currentUserId
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["GroupTypeId"] = new SelectList(_context.GroupType, "Id", "Id", @group.GroupTypeId);
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Group @group = await _context.Group.SingleOrDefaultAsync(m => m.Id == id);


            if (@group == null)
            {
                return NotFound();
            }

            GroupViewModel groupViewModel = new GroupViewModel()
            {
                Description = group.Description,
                Name = group.Name,
                Id = group.Id,
                GroupTypeId = group.GroupTypeId,
                PhotoIsExist = group.Photo != null,
                NumberOfMember = group.NumberOfMember,
                NumberOfViewer = group.NumberOfViewer
            };

            ViewData["GroupTypeId"] = new SelectList(_context.GroupType, "Id", "Id", groupViewModel.GroupTypeId);
            return View(groupViewModel);
        }

        // POST: Groups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,GroupTypeId,Description,Photo")] GroupViewModel @group, IFormFile Photo)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }

            string currentUserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                Group updatedGroup = _context.Group.SingleOrDefault(g => g.Id == id);

                if (updatedGroup == null)
                {
                    return NotFound();
                }

                updatedGroup.Updated = DateTime.Now;
                updatedGroup.Description = @group.Description;
                updatedGroup.LastUpdatedBy = currentUserId;
                updatedGroup.GroupTypeId = @group.GroupTypeId;
                updatedGroup.Name = @group.Name;


                if (Photo != null && Photo.Length != 0)
                {

                    using (MemoryStream ms = new MemoryStream())
                    {
                        Photo.CopyTo(ms);
                        byte[] fileBytes = ms.ToArray();

                        updatedGroup.Photo = fileBytes;
                    }
                }

                _context.Update(updatedGroup);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["GroupTypeId"] = new SelectList(_context.GroupType, "Id", "Id", @group.GroupTypeId);
            return View(@group);
        }


        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Group @group = await _context.Group
                .Include(g => g.GroupType)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Group @group = await _context.Group.SingleOrDefaultAsync(m => m.Id == id);
            _context.Group.Remove(@group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(string id)
        {
            return _context.Group.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Viewer(string id)
        {
            int numberOfViewers = 0;
            string currentUserId = _userManager.GetUserId(User);
            Group group = await _context.Group.SingleAsync(u => u.Id == id);
            GroupViewers viewer = await _context.GroupViewers.SingleOrDefaultAsync(g => g.ViewersId == currentUserId && g.GroupId == id);

            if (viewer == null)//check if current user view the curreent group before
            {
                group.NumberOfViewer += 1;
                _context.GroupViewers.Add(new GroupViewers()
                {
                    ViewersId = currentUserId,
                    GroupId = id
                });
            }
            _context.Entry(group).State = EntityState.Modified;
            numberOfViewers = group.NumberOfViewer;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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

        private async Task<IActionResult> ConfirmRequestToJoinGroup(string id, string userId)
        {
            int numberOfFollwer = 0;

            Group group = await _context.Group.SingleAsync(u => u.Id == id);
            GroupMember follower = await _context.GroupMember.SingleOrDefaultAsync(u => u.GroupId == id && u.MemberId == userId);

            if (follower == null)//check if user follow the curreent group before
            {
                group.NumberOfMember += 1;
                _context.GroupMember.Add(new GroupMember()
                {
                    GroupId = id,
                    IsAdmin = false,
                    JoinDate = DateTime.Now,
                    MemberId = userId
                });

                numberOfFollwer = group.NumberOfMember;
                _context.Entry(group).State = EntityState.Modified;


                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            return Ok();
        }


        public async Task<IActionResult> Follow(string id)
        {
            int numberOfFollwer = 0;
            string currentUserId = _userManager.GetUserId(User);
            Group group = await _context.Group.SingleAsync(u => u.Id == id);
            GroupMember follower = await _context.GroupMember.SingleOrDefaultAsync(u => u.GroupId == id && u.MemberId == currentUserId);

            if (follower == null)//check if current user follow the curreent group before
            {
                group.NumberOfMember += 1;
                _context.GroupMember.Add(new GroupMember()
                {
                    GroupId = id,
                    IsAdmin = false,
                    JoinDate = DateTime.Now,
                    MemberId = currentUserId
                });
            }
            else
            {
                group.NumberOfMember -= 1;
                _context.GroupMember.Remove(follower);
            }

            numberOfFollwer = group.NumberOfMember;
            _context.Entry(group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(numberOfFollwer);
        }


        public async Task<IActionResult> ChangeAdministrationPermisstion(string id, string userId)
        {
            string currentUserId = _userManager.GetUserId(User);

            GroupMember currentFollower = await _context.GroupMember.SingleOrDefaultAsync(u => u.GroupId == id && u.MemberId == currentUserId);

            GroupMember follower = await _context.GroupMember.SingleOrDefaultAsync(u => u.GroupId == id && u.MemberId == userId);

            //Check if current user have role to change other members permission. In addition, the selected follower is member of the current group
            if (currentFollower != null && currentFollower.IsAdmin && follower != null)
            {
                follower.IsAdmin = !(follower.IsAdmin);

                _context.Entry(follower).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Ok();
        }

        public List<GroupMemberRequest> GenerateEmailConfirmationRequest(string groupId, string fromUserId, List<string> emailList)
        {

            List<GroupMemberRequest> requestList = new List<GroupMemberRequest>();

            foreach (string email in emailList)
            {
                string gId = Guid.NewGuid().ToString();

                GroupMemberRequest newRequest = new GroupMemberRequest()
                {
                    FromId = fromUserId,
                    GroupId = groupId,
                    RequestDate = DateTime.Now,
                    ToEmail = email,
                    Code = Code.Hash($"{gId}-{groupId}-{fromUserId}-{email}"),
                    Id = gId
                };


                requestList.Add(newRequest);
            }


            _context.GroupMemberRequest.AddRange(requestList);
            _context.SaveChanges();


            return requestList;
        }

        public bool ValidateRequestToken(string groupId, string email, string code)
        {
            if (code == null)
            {
                return false;
            }

            GroupMemberRequest requestInfo = _context.GroupMemberRequest.SingleOrDefault(g => g.Code == code);

            //Check if code is exist, and the requested user is current login, and request group is equal the access one
            if (requestInfo == null || requestInfo.ToEmail != email || requestInfo.GroupId != groupId)
            {
                return false;
            }

            try
            {
                //remove the request, to ensure that will not used again.
                _context.Entry(requestInfo).State = EntityState.Deleted;
                _context.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string groupId, string email, string code)
        {
            if (email == null || code == null || !ValidateRequestToken(groupId, email, code))
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return Content($"This message confirm that you are not logint to the Link Social Network. You need to login using '{email}' email, after that click on the requested link sent to '{email}' email.");
            }

            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            string currentUserId = _userManager.GetUserId(User);
            if (user == null || user.Id != currentUserId)
            {
                return Content($"Unable to load user with Email '{email}'.");
            }

            //add user to follower members
            await ConfirmRequestToJoinGroup(groupId, user.Id);

            return RedirectToAction(nameof(GroupsController.Details), "Groups", new { id = groupId });
        }

        public async Task<IActionResult> SendRequest(string id, string emailStr)
        {
            string groupId = id;
            List<string> emailList = JsonConvert.DeserializeObject<List<string>>(emailStr);

            if (groupId == null || emailList == null)
            {
                return NotFound();
            }

            if (emailList.Count() >= 1)
            {
                Group groupInfo = await _context.Group.SingleOrDefaultAsync(g => g.Id == groupId);
                if (groupInfo != null)
                {
                    string currentUserId = _userManager.GetUserId(User);

                    List<GroupMemberRequest> requestList = GenerateEmailConfirmationRequest(groupId, currentUserId, emailList);

                    foreach (GroupMemberRequest request in requestList)
                    {
                        string callbackUrl = Url.EmailConfirmationGroupLink(groupId, request.ToEmail, request.Code, Request.Scheme);

                        string emailMessage = $@"<p> <b>Hello,</b> </p> 
                                         <p> '{User.Identity.Name}' has been sent to you a request  to join '{groupInfo.Name}' group </p> 
                                        <p>Please click on the link below to apply the request</p>
                                        <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a></p> 
                                        <p>Thanks</p> ";

                        await _emailSender.SendEmailAsync(request.ToEmail, $"Request to join '{groupInfo.Name}' group ", emailMessage);

                        _emailLogs.Add(request.ToEmail, "Request to join ", emailMessage, EmailType.GroupRequest, true);
                    }

                }
            }
            return Ok();
        }



    }
}
