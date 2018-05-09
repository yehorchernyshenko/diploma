using System;
using System.ComponentModel.DataAnnotations;
using Diploma.Models.Enums;

namespace Diploma.Models.ViewModels
{
    public class CreateRouteViewModel
    {
        public string From { get; set; }

        public string To { get; set; }

        public int Price { get; set; } = 0;

        [DataType(DataType.Time)]
        [Display(Name = "Time")]
        public DateTime DepartureTime { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime DepartureDate { get; set; } = DateTime.Now;

        public Currency Currency { get; set; }

        [Display(Name = "Route Length")]
        public double RouteLength { get; set; } = 0;

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }
}