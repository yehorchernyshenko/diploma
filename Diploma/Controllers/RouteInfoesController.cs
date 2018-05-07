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

        public async Task<IActionResult> Index()
        {
            var routeInfoesList = await _context.RouteInfo.ToListAsync();

            return View(routeInfoesList);
        }

        public async Task<IActionResult> Details(Guid? id)
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
                    RouteLength = addRouteViewModel.RouteLength,
                    Price = addRouteViewModel.Price,
                    RouteStatus = RouteStatus.Pending,
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

                return RedirectToAction("Index");
            }
            return View(addRouteViewModel);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeInfo = await _context.RouteInfo.SingleOrDefaultAsync(m => m.Id == id);

            if (routeInfo == null)
            {
                return NotFound();
            }

            return View(routeInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,IsDriver,IsPassenger")] RouteInfo routeInfo)
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
                return RedirectToAction(nameof(Index));
            }
            return View(routeInfo);
        }

        public async Task<IActionResult> Delete(Guid? id)
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
        public async Task<IActionResult> Delete(Guid id)
        {
            var routeInfo = await _context.RouteInfo.SingleOrDefaultAsync(m => m.Id == id);
            _context.RouteInfo.Remove(routeInfo);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool RouteInfoExists(Guid id)
        {
            return _context.RouteInfo.Any(e => e.Id == id);
        }
    }
}
