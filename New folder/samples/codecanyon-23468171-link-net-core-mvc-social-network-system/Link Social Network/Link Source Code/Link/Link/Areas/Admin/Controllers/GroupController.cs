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

    public class GroupController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GroupController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// loading dataInfo from db collection to dataInfotable
        /// for more detail
        /// https://www.c-sharpcorner.com/article/using-jquery-dataInfotables-grid-with-asp-net-core-mvc/
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

                // getting all user dataInfo  
                var dataInfo = _context
                    .Group.Include(o => o.GroupType)
                    .Join(_context.Users, g => g.Creator, u => u.Id, (group, user) => new
                    {
                        group.Id,
                        group.Name,
                        group.Created,
                        CreatedStr = group.Created == null ? "" : group.Created.ToString("MM/dd/yyyy hh:mm tt"),
                        group.NumberOfMember,
                        group.NumberOfViewer,
                        Creator = user.UserName,
                        GroupType = group.GroupType.Name,
                        Description= group.Description ?? ""

                    });

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    dataInfo = dataInfo.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    dataInfo = dataInfo.Where(m => m.Creator.Contains(searchValue) ||
                                                   m.Name.Contains(searchValue) ||
                                                   m.Description.Contains(searchValue) ||
                                                   m.CreatedStr.Contains(searchValue) ||
                                                   m.GroupType.Contains(searchValue) ||
                                                   m.NumberOfMember.ToString() == searchValue ||
                                                   m.NumberOfViewer.ToString() == searchValue);
                }

                //total number of rows counts   
                recordsTotal = dataInfo.Count();
                //Paging   
                var data = dataInfo.Skip(skip).Take(pageSize).ToList();

                //Returning Json data  
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