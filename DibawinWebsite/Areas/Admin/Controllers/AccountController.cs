﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using DibawinWebsite.Models;
using DibawinWebsite.Areas.Identity.Data;
using DibawinWebsite.ClassLibraries;
using DibawinWebsite.ClassLibraries.NotificationHandler;
using DibawinWebsite.Models.ViewModels;
using DibawinWebsite.Repository;
using DibawinWebsite.ClassLibraries.MenuGenrator.Attributes;

namespace DibawinWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperVisor")]
    [MenuItem(Action = "Index",Title = "کاربران", CssIcon = "fa fa-user",Order =2)]
    public class AccountController : Controller
    {
        #region Inject
        private readonly IHostingEnvironment _hostingEnvironment;
        readonly string contentRootPath;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;
        DbRepository<MyDBContext, UserModified, int> _dbUserModified;
        DbRepository<MyDBContext, UserImage, int> _dbUserImage;
        public AccountController
            (
                IHostingEnvironment hostingEnvironment,
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                RoleManager<IdentityRole> roleManager,
                DbRepository<MyDBContext, UserModified, int> dbUserModified,
                DbRepository<MyDBContext, UserImage, int> dbUserImage
            )
        {
            _hostingEnvironment = hostingEnvironment;
            contentRootPath = _hostingEnvironment.ContentRootPath;//returns the root path of the website
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbUserModified = dbUserModified;
            _dbUserImage = dbUserImage;

        }
        #endregion
        #region Index
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("SignIn", "Account");
        }
        #endregion
        #region InitializeUser
        //Initialize User and Roles--Start
        [AllowAnonymous]
        public async Task<IActionResult> Initialize()
        {
            string MainAdmin = "admin@mail.com";
            string nvm;
            //making identity roles
            string[] RolesName = { "Admin", "SuperVisor", "Customer" };
            foreach (var item in RolesName)
            {
                if (await _roleManager.RoleExistsAsync(item) == false)
                {
                    IdentityRole role = new IdentityRole(item);
                    await _roleManager.CreateAsync(role);
                }
            }
            //This command controls whether or not Admin user exists. If not, it will make it.
            var AdminExist = await _userManager.FindByNameAsync(MainAdmin);
            if (AdminExist == null)
            {
                ApplicationUser admin = new ApplicationUser()
                {
                    UserName = MainAdmin,
                    Email = MainAdmin,
                    FirstName = "Saeed",
                    LastName = "Panahi",
                    Gendre = 1,
                    DateOfBirth = new DateTime(1990, 1, 20),
                    SpecialUser = true,
                    Rank = 1000,
                    NationalCode = "111111111",
                    PhoneNumber = "09386723416",
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true,
                    Status = true,

                };
                var status = await _userManager.CreateAsync(admin, "Qwerty@01234");
                if (status.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            nvm = NotificationHandler.SerializeMessage<string>(NotificationHandler.Success_Insert, contentRootPath);
            return RedirectToAction("SignIn", new { notification = nvm });
        }
        //Initialize User and Roles--End
        #endregion
        #region SignIn
        [AllowAnonymous]
        public IActionResult SignIn(string notification)
        {
            if (notification != null)
            {
                ViewData["nvm"] = NotificationHandler.DeserializeMessage(notification);
                return View();
            }
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> SignInConfirm(LoginViewModel model)
        {
            string nvm;
            if (!ModelState.IsValid)
            {
                nvm = NotificationHandler.SerializeMessage<string>(NotificationHandler.Wrong_Values, contentRootPath);
                return RedirectToAction("SignIn", new { notification = nvm });
            }
            //Model is Valid
            var CurrentUser = await _userManager.FindByNameAsync(model.UserName);
            if (CurrentUser == null)
            {
                nvm = NotificationHandler.SerializeMessage<string>(NotificationHandler.Failed_Login, contentRootPath);
                return RedirectToAction("Signin", new { notification = nvm });
            }
            //User Exists
            var CurrentUserStatus = CurrentUser.Status;
            var CurrentUserRole = await _userManager.IsInRoleAsync(CurrentUser, "Admin");
            if (!CurrentUserStatus)
            {
                nvm = NotificationHandler.SerializeMessage<string>(NotificationHandler.Failed_Login, contentRootPath);
                return RedirectToAction("SignIn", new { notification = nvm });
            }
            if (!CurrentUserRole)
            {
                nvm = NotificationHandler.SerializeMessage<string>(NotificationHandler.Failed_Login, contentRootPath);
                return RedirectToAction("SignIn", new { notification = nvm });
            }
            var status = await _signInManager.PasswordSignInAsync(CurrentUser, model.Password, model.RememberMe, false);
            if (status.Succeeded)
            {
                nvm = NotificationHandler.SerializeMessage<string>(NotificationHandler.Success_Login, contentRootPath);
                return RedirectToAction("Index", "Home", new { notification = nvm }); //successful signin
            }
            nvm = NotificationHandler.SerializeMessage<string>(NotificationHandler.Failed_Login, contentRootPath);
            return RedirectToAction("Signin", new { notification = nvm });
        }
        #endregion
        #region Signout
        public async Task<IActionResult> Signout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Signin");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            } 
        }
        #endregion

        #region Signup
        [MenuItem(Title = "ثبت کاربر")]
        public ViewResult Signup() => View();
        #endregion

        [MenuItem(Title ="لیست کاربران")]
        public async Task<IActionResult> UserList(string notification)
        {
            var dbViewModel =  _userManager.Users.ToList();
            if (notification != null)
            {
                ViewData["nvm"] = NotificationHandler.DeserializeMessage(notification);
            }
            return View(dbViewModel);
        }
    }
}