/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Link.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "ADMIN")]

    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// loading data from db collection to datatable
        /// for more detail
        /// https://www.c-sharpcorner.com/article/using-jquery-datatables-grid-with-asp-net-core-mvc/
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;
                // getting all user data  

                var currentCommentList = _context.Comment.Include(o => o.CommentGroupType)
                   .Where(c => c.Parent == null);

                //total number of rows counts   
                recordsTotal = currentCommentList.Count();

                currentCommentList = currentCommentList.OrderByDescending(c => c.Created)
                   .Skip(skip).Take(pageSize);

                var dataInfo = (from comment in currentCommentList
                                join @group in _context.Group on comment.GroupId equals @group.Id
                                join user in _context.Users on comment.Creator equals user.Id
                                select new
                                {
                                    comment.Content,
                                    Created = comment.Created.ToString("MM/dd/yyyy hh:mm tt"),
                                    Creator = user.UserName,
                                    comment.Id,
                                    comment.UpvoteCount,
                                    CommentGroupType = comment.CommentGroupType.Name,
                                    Group = @group.Name

                                }).ToList().Union(from comment in currentCommentList
                                                  join @group in _context.Users on comment.GroupId equals @group.Id
                                                  join user in _context.Users on comment.Creator equals user.Id
                                                  select new
                                                  {
                                                      comment.Content,
                                                      Created = comment.Created.ToString("MM/dd/yyyy hh:mm tt"),
                                                      Creator = user.UserName,
                                                      comment.Id,
                                                      comment.UpvoteCount,
                                                      CommentGroupType = comment.CommentGroupType.Name,
                                                      Group = user.UserName
                                                  });

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    //  currentCommentList = currentCommentList.OrderBy(sortColumn + " " + sortColumnDirection);
                    switch (sortColumn)
                    {
                        case "Content":
                            {
                                if (sortColumnDirection == "asc")
                                {
                                    dataInfo = dataInfo.OrderBy(o => o.Content);

                                }
                                else
                                {
                                    dataInfo = dataInfo.OrderByDescending(o => o.Content);
                                }

                            }
                            break;
                        case "Created":
                            {
                                if (sortColumnDirection == "asc")
                                {
                                    dataInfo = dataInfo.OrderBy(o => o.Created);

                                }
                                else
                                {
                                    dataInfo = dataInfo.OrderByDescending(o => o.Created);
                                }

                            }
                            break;
                        case "Creator":
                            {
                                if (sortColumnDirection == "asc")
                                {
                                    dataInfo = dataInfo.OrderBy(o => o.Creator);

                                }
                                else
                                {
                                    dataInfo = dataInfo.OrderByDescending(o => o.Creator);
                                }
                            }
                            break;

                        case "UpvoteCount":
                            {
                                if (sortColumnDirection == "asc")
                                {
                                    dataInfo = dataInfo.OrderBy(o => o.UpvoteCount);

                                }
                                else
                                {
                                    dataInfo = dataInfo.OrderByDescending(o => o.UpvoteCount);
                                }

                            }
                            break;
                        case "CommentGroupType":
                            {
                                if (sortColumnDirection == "asc")
                                {
                                    dataInfo = dataInfo.OrderBy(o => o.CommentGroupType);

                                }
                                else
                                {
                                    dataInfo = dataInfo.OrderByDescending(o => o.CommentGroupType);
                                }

                            }
                            break;
                        case "Group":
                            {
                                if (sortColumnDirection == "asc")
                                {
                                    dataInfo = dataInfo.OrderBy(o => o.Group);

                                }
                                else
                                {
                                    dataInfo = dataInfo.OrderByDescending(o => o.Group);
                                }

                            }
                            break;
                    }
                }

                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    dataInfo = dataInfo.Where(m => m.Creator.Contains(searchValue) ||
                                                   m.Group.Contains(searchValue) ||
                                                   m.Content.Contains(searchValue) ||
                                                   m.Creator.Contains(searchValue) ||
                                                   m.CommentGroupType.Contains(searchValue) ||
                                                   m.UpvoteCount.ToString() == searchValue);
                }


                //Paging   
                var data = dataInfo.ToList();
                //Returning Json Data  
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = data
                });

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}