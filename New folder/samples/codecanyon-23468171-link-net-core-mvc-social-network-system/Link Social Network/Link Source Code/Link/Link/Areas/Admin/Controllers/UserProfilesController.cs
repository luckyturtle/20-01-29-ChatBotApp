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

    public class UserProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserProfilesController(ApplicationDbContext context)
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
                var userData = _context.Users.Include(o => o.Gender).Select(user => new
                {
                    user.UserName,
                    user.Id,
                    user.NumberOfFollwer,
                    user.NumberOfViewer,
                    PhoneNumber = user.PhoneNumber ?? "",
                    user.Email,
                    Gender = user.Gender == null ? "" : user.Gender.Name,
                    RegestrationDate = user.RegistrationDate == null ? "" : user.RegistrationDate.ToShortDateString(),
                    //  Description = user.Description ?? "",
                    BirthDate = user.BirthDate == null ? "" : user.BirthDate.ToString(),
                    // SocialLink = user.SocialMedia ?? ""
                });

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    userData = userData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    userData = userData.Where(m => m.UserName.Contains(searchValue) ||
                                                   m.Email.Contains(searchValue) ||
                                                   m.Gender.Contains(searchValue) ||
                                                   m.PhoneNumber.Contains(searchValue) ||
                                                   m.NumberOfFollwer.ToString() == searchValue ||
                                                   m.NumberOfViewer.ToString() == searchValue);
                }

                //total number of rows counts   
                recordsTotal = userData.Count();
                //Paging   
                var data = userData.Skip(skip).Take(pageSize).ToList();
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