/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Models;
using Link.Models.SearchViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Mime;

namespace Link.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult FindComments(MainSearchViewModel model)
        {
            //*=> mean all
            if (!string.IsNullOrEmpty(model.MainContian))
            {
                model.Contain = model.MainContian;
            }
            else
            {
                model.Contain = model.Contain ?? "*";//* = all
            }

            model.CreateBy = model.CreateBy ?? "*";
            model.FromDate = model?.FromDate ?? "*";
            model.ToDate = model?.ToDate ?? "*";
            model.Group = model.Group ?? "*";

            return View(model);
        }

        public IActionResult Hashtag(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            ViewData["UserId"] = currentUserId;
            ViewData["id"] = id;
            ViewData["tag"] = id;

            return View();
        }

        [Route("[controller]/Comment/{id}/{group}/{type}")]
        public IActionResult Comment(string id, string group, int type)
        {
            var currentUserId = _userManager.GetUserId(User);
            ViewData["UserId"] = currentUserId;
            ViewData["id"] = id;
            ViewData["groupId"] = group;
            ViewData["type"] = type;

            return View();
        }

        public IActionResult DownloadAttachment(string id)
        {
            Models.CommentModels.Comment comment = _context.Comment.SingleOrDefault(m => m.Id == id);

            try
            {
                if (comment.FileMimeType != null)
                {
                    ContentDisposition cd = new ContentDisposition
                    {
                        FileName = comment.FileURL,
                        Inline = true
                    };
                    Response.Headers["Content-Disposition"] = cd.ToString();
                    return File(comment.File, comment.FileMimeType);
                }
                else
                {
                    return File(comment.File, MediaTypeNames.Application.Octet, comment.FileURL);

                }
            }

            catch (InvalidOperationException)
            {
                return File(comment.File, MediaTypeNames.Application.Octet, comment.FileURL);
            }

        }
    }
}