using System.Collections.Generic;
using Diploma.Models.Entities;

namespace Diploma.Models.ViewModels
{
    public class RouteDetailsViewModel
    {
        public RouteInfo RouteInfo { get; set; }

        public int AvailablePlaces { get; set; }

        public List<RouteInfo> PassengersApplications { get; set; }
    }
}