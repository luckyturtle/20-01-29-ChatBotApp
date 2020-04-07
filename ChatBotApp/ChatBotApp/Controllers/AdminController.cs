using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotApp.Controllers
{//[Authorize(Roles="Administrators")]
    [Authorize]//(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("[controller]/[action]")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}