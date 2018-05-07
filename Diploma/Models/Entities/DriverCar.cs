using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diploma.Models.Entities
{
    public class DriverCar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public User User { get; set; }

        public string CarModel { get; set; }

        public DateTime LastModificationDate = DateTime.Now;
    }
}