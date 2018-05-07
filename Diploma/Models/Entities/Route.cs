using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Diploma.Models.Enums;

namespace Diploma.Models.Entities
{
    public class Route
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public Currency Currency { get; set; }

        public DateTime LastModificationDate => DateTime.Now;

        public RouteStatus RouteStatus { get; set; }

        public virtual DriverCar Car { get; set; }

        public double RouteLength { get; set; }
    }
}