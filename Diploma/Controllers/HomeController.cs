using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Diploma.Models;
using Diploma.Models.ViewModels;
using Diploma.Services;
using Microsoft.AspNet.Identity;

namespace Diploma.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApplicationService _applicationService;

        public HomeController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LeaveMessage(LeaveMessageViewModel leaveMessageViewModel)
        {
            if (ModelState.IsValid) {
                var userId = User.Identity.GetUserId();

                leaveMessageViewModel.UserId = userId;

                await _applicationService.LeaveMessage(leaveMessageViewModel);

                ViewBag.Message = "Thank you for your message!\n We`ll contact you in 24 hours.";
            }

            return View("Contact");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
