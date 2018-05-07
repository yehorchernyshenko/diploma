﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Route> Routes { get; set; }

        public DbSet<RouteInfo> RouteInfo { get; set; }

        public DbSet<DriverCar> DriverCars { get; set; }
    }
}