using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Models;
using Diploma.Models.Entities;
using Diploma.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public AccountController(Microsoft.AspNetCore.Identity.UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationContext context,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel {ReturnUrl = returnUrl});
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.User.FirstOrDefault(item => item.Email == model.Email);

                if (user != null && user.IsBlocked == true)
                {
                    return RedirectToAction("Error", "Home",
                        new ErrorViewModel
                        {
                            ErrorMessage =
                                "Sorry, your account is blocked. Please use contact us form in order to unblock it."
                        });
                }

                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Wrong email or password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AccountDetails()
        {
            var userId = User.Identity.GetUserId();

            var user = await _context.User.FirstAsync(item => item.Id == userId);

            var accountDetailsViewModel = _mapper.Map<User, AccountDetailsViewModel>(user);

            return View("AccountDetailsEdit", accountDetailsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UserAccountDetails(string userId)
        {
            var user = await _context.User.FirstAsync(item => item.Id == userId);

            var accountDetailsViewModel = _mapper.Map<User, AccountDetailsViewModel>(user);

            return View("UserAccountDetails", accountDetailsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccountDetails(AccountDetailsViewModel accountDetailsViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.User.FirstAsync(item => item.Id == User.Identity.GetUserId());

                user.FirstName = accountDetailsViewModel.FirstName;
                user.LastName = accountDetailsViewModel.LastName;
                user.Email = accountDetailsViewModel.Email;
                user.FacebookProfileLink = accountDetailsViewModel.FacebookProfileLink;
                user.LinkedinProfileLink = accountDetailsViewModel.LinkedinProfileLink;
                user.BirthdayDate = accountDetailsViewModel.BirthdayDate;
                user.PhoneNumber = accountDetailsViewModel.PhoneNumber;
                user.SecurityStamp = Guid.NewGuid().ToString();

                await _userManager.UpdateAsync(user);

                ViewBag.UpdateSuccessful = true;
            }

            return View("AccountDetailsEdit", accountDetailsViewModel);
        }
    }
}