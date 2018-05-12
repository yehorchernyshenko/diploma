using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Diploma.Models.Enums;

namespace Diploma.Models.Entities
{
    public class RouteInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual User User { get; set; }

        public virtual Route Route { get; set; }

        public bool IsDriver { get; set; }

        public bool IsPassenger { get; set; }

        public RouteApplicationStatus RouteApplicationStatus { get; set; }
    }
}