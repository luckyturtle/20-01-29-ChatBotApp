using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.data;

namespace ChatBotApp.Controllers
{
    public static class Extensions
    {
        public static string GetDomain(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(0, stop) : string.Empty;
        }

        public static string GetLogin(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(stop + 1, s.Length - stop - 1) : string.Empty;
        }
    }

    [Authorize]//(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    var user = _userManager.Users.Where(u => u.Email.Equals(model.Email)).SingleOrDefault(); //changed by bashar Developer
                    if (user == null)
                    {
                        user = _userManager.Users.Where(u => u.UserName.Equals(model.Email)).SingleOrDefault(); //changed by bashar Developer
                    }
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(model);
                    }
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        /*
                        //Create a Cookie with a suitable Key.
                        HttpCookie loginCookie = new HttpCookie("Login");
                        //Set the Cookie value.
                        loginCookie.Values["Name"] = txtName.Text;
                        loginCookie.Values["Password"] = txtPassword.Text;
                        loginCookie.Path = Request.ApplicationPath;
                        //Set the Expiry date.
                        loginCookie.Expires = DateTime.Now.AddDays(1);
                        //Add the Cookie to Browser.
                        Response.Cookies.Add(loginCookie);*/

                        //if (user.NormalizedUserName.Equals("ADMIN")) return RedirectToAction(nameof(AdminController.Index), "Admin"); //returnUrl = "~/admin/index";
                        //else 
                        return RedirectToAction(nameof(ChatController.Index), "Chat"); //returnUrl = "~/chat/index";
                        //return RedirectToLocal(returnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        //_logger.LogWarning("User account locked out.");
                        return RedirectToAction(nameof(Lockout));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(model);
                    }
                }

                // If we got this far, something failed, redisplay form 
                return View(model);
            }
            catch (Exception ex)
            {
                string ss = ex.ToString();
                return View();
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var isCurrentNameIsExist = _userManager.Users.Where(u => u.UserName.Equals(model.Username)).Any();

                if (isCurrentNameIsExist)
                {
                    ModelState.AddModelError(string.Empty, $" The current user name '{model.Username}' is exist.");
                    return View(model);
                }

                var isCurrentEmailIsExist = _userManager.Users.Where(u => u.Email.Equals(model.Email)).Any();

                if (isCurrentEmailIsExist)
                {
                    ModelState.AddModelError(string.Empty, $" The current user email '{model.Email}' is exist.");
                    return View(model);
                }


                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, CreatedDate = DateTime.Now };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //please write user add code
                    return RedirectToLocal(returnUrl);
                }
                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            //_logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }
        }

        #endregion
    }
}