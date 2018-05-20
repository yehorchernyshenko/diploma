using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Diploma.Models;
using Diploma.Models.Entities;
using Diploma.Models.Enums;
using Diploma.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNet.Identity;

namespace Diploma.Controllers
{
    [Authorize]
    public class RouteInfoesController : Controller
    {
        private readonly ApplicationContext _context;

        public RouteInfoesController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetAvailableRoutesList()
        {
            var currentUserId = User.Identity.GetUserId();

            var appliedUserRouteInfoesId = await GetUserAppliedRoutesIds();

            var routeInfoesList = await _context.RouteInfo
                .Include(context => context.User)
                .Include(context => context.Route)
                .Where(routeInfo => routeInfo.User.Id != currentUserId
                        && routeInfo.IsDriver == true
                        && !appliedUserRouteInfoesId.Contains(routeInfo.Route.Id))
                .ToListAsync();

            return View("Index", routeInfoesList);
        }

        private async Task<List<Guid>> GetUserAppliedRoutesIds()
        {
            var currentUserId = User.Identity.GetUserId();

            var appliedUserRouteInfoesId = await _context.RouteInfo
                .Include(context => context.User)
                .Include(context => context.Route)
                .Where(routeInfo => routeInfo.User.Id == currentUserId
                        && routeInfo.IsPassenger == true)
                 .Select(item => item.Route.Id)
                .ToListAsync();

            return appliedUserRouteInfoesId;
        }

        public async Task<IActionResult> GetUserAppliedRoutesList()
        {
            var appliedUserRouteInfoesId = await GetUserAppliedRoutesIds();

            var appliedUserRouteInfoesList = await _context.RouteInfo
                .Include(context => context.User)
                .Include(context => context.Route)
                .Where(routeInfo => appliedUserRouteInfoesId.Contains(routeInfo.Route.Id)
                                    && routeInfo.IsPassenger == true)
                .ToListAsync();

            return View("UserAppliedRoutes", appliedUserRouteInfoesList);
        }

        public async Task<IActionResult> GetUserCreatedRoutesList()
        {
            var currentUserId = User.Identity.GetUserId();

            var userRouteInfoesList = await _context.RouteInfo
                .Include(context => context.User)
                .Include(context => context.Route)
                .Where(routeInfo => routeInfo.User.Id == currentUserId
                        && routeInfo.IsDriver == true
                        && routeInfo.RouteApplicationStatus != RouteApplicationStatus.Cancelled)
                .ToListAsync();

            return View("UserCreatedRoutes", userRouteInfoesList);
        }

        public async Task<IActionResult> ApplyForRoute(Guid? routeId)
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUser = _context.Users.FirstOrDefault(user => user.Id == currentUserId);

            var appliedRoute = _context.Routes.FirstOrDefault(route => route.Id == routeId);

            var routeInfo = new RouteInfo
            {
                IsPassenger = true,
                User = currentUser,
                Route = appliedRoute,
                RouteApplicationStatus = RouteApplicationStatus.Pending
            };

            _context.RouteInfo.Add(routeInfo);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetUserAppliedRoutesList");
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeInfo = await _context.RouteInfo
                .Include(context => context.User)
                .Include(context => context.Route)
                .SingleOrDefaultAsync(routeInfoItem => routeInfoItem.Id == id);

            if (routeInfo == null)
            {
                return NotFound();
            }

            var routeDetailsViewModel = new RouteDetailsViewModel
            {
                RouteInfo = routeInfo,
                AvailablePlaces = routeInfo.Route.Capacity - await GetCountOfAppliedUsers(routeInfo.Route.Id),
                PassengersApplications = await GetPassengersRouteApplicationsList(routeInfo.Route.Id)
            };  

            return View(routeDetailsViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRouteViewModel addRouteViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                var currentUser = _context.Users.FirstOrDefault(user => user.Id == currentUserId);

                var route = new Route
                {
                    Currency = addRouteViewModel.Currency,
                    From = addRouteViewModel.From,
                    To = addRouteViewModel.To,
                    RouteLength = addRouteViewModel.RouteLength.Value,
                    Price = addRouteViewModel.Price,
                    RouteStatus = RouteStatus.Pending,
                    Capacity = addRouteViewModel.Capacity,
                    Comment = addRouteViewModel.Comment,
                    DepartureTime = new DateTime(addRouteViewModel.DepartureDate.Year,
                        addRouteViewModel.DepartureDate.Month, addRouteViewModel.DepartureDate.Day,
                        addRouteViewModel.DepartureTime.Hour, addRouteViewModel.DepartureTime.Minute, 0)
                };

                var routeInfo = new RouteInfo
                {
                    User = currentUser,
                    IsDriver = true,
                    Route = route
                };

                _context.Add(routeInfo);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            return View(addRouteViewModel);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeInfo = await _context.RouteInfo
                .Include(item => item.Route)
                .Include(item => item.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (routeInfo == null)
            {
                return NotFound();
            }

            var currentUserId = User.Identity.GetUserId();

            if (routeInfo.User.Id != currentUserId)
            {
                return View("Error", new ErrorViewModel{ErrorMessage = "You can`t edit not your route!"});
            }

            return View(routeInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassengerApplicationStatus(Guid userId, Guid routeId, RouteApplicationStatus routeApplicationStatus)
        {
            var routeInfo = await _context.RouteInfo
                .Include(item => item.User)
                .Include(item => item.Route)
                .Where(item => item.Route.Id == routeId
                               && item.User.Id == userId.ToString()).SingleAsync();

            routeInfo.RouteApplicationStatus = routeApplicationStatus;

            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RouteInfo routeInfo)
        {
            if (id != routeInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routeInfo);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteInfoExists(routeInfo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(routeInfo);
        }

        public async Task<IActionResult> Cancel(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeInfo = await _context.RouteInfo
                .SingleOrDefaultAsync(m => m.Id == id);
            if (routeInfo == null)
            {
                return NotFound();
            }

            return View(routeInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var routeInfo = await _context.RouteInfo
                .Include(item => item.User)
                .Include(item => item.Route)
                .SingleOrDefaultAsync(m => m.Id == id);

            var currentUserId = User.Identity.GetUserId();

            if (routeInfo.User.Id != currentUserId)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "You can`t delete not your route!" });
            }

            var countOfAppliedUsers = await GetCountOfAppliedUsers(routeInfo.Route.Id);
            var appliedPassengersRoutesList = await GetAppliedPassengersList(routeInfo.Route.Id);
            appliedPassengersRoutesList.Add(routeInfo);

            if (countOfAppliedUsers > 1)
            {
                foreach (var routeInfoItem in appliedPassengersRoutesList) {
                    routeInfoItem.RouteApplicationStatus = RouteApplicationStatus.Cancelled;
                }
            } else {
                _context.RouteInfo.Remove(routeInfo);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        private async Task<List<RouteInfo>> GetAppliedPassengersList(Guid routeId)
        {
            var passengersRoutesList = await _context.RouteInfo
                .Include(context => context.Route)
                .Where(routeInfoItem =>
                    routeInfoItem.IsPassenger == true
                    && routeInfoItem.Route.Id == routeId
                    && routeInfoItem.RouteApplicationStatus == RouteApplicationStatus.Approved)
                .ToListAsync();

            return passengersRoutesList;
        }

        private async Task<int> GetCountOfAppliedUsers(Guid routeId)
        {
            var approvedPassengerList = await GetAppliedPassengersList(routeId);

            return approvedPassengerList.Count;
        }

        private async Task<List<RouteInfo>> GetPassengersRouteApplicationsList(Guid routeId)
        {
            var passengersRouteApplicationsList = await _context.RouteInfo
                .Include(context => context.User)
                .Include(context => context.Route)
                .Where(routeInfo => routeInfo.Route.Id == routeId
                                      && routeInfo.IsPassenger == true)
                .ToListAsync();

            return passengersRouteApplicationsList;
        }

        private bool RouteInfoExists(Guid id)
        {
            return _context.RouteInfo.Any(e => e.Id == id);
        }
    }
}
