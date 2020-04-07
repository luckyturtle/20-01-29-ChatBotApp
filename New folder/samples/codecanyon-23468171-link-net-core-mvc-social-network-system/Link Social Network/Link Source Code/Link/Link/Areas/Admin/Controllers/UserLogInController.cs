/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Models.UserLogsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Link.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "ADMIN")]

    public class UserLogInController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserLogInController(ApplicationDbContext context)
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
                    .UserLogs.Where(o => o.UserActionId == UserAction.LogIn)
                    .Join(_context.Users, log => log.UserId, u => u.Id, (log, user) => new
                    {
                        log.Id,
                        user.UserName,
                        log.UserAgent,
                        LoginDateTime = log.ActionDate == null ? "" : log.ActionDate.ToString("MM/dd/yyyy hh:mm tt"),
                    });

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    dataInfo = dataInfo.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    dataInfo = dataInfo.Where(m => m.UserName.Contains(searchValue) ||
                                                   m.UserAgent.Contains(searchValue) ||
                                                   m.LoginDateTime.Contains(searchValue));

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