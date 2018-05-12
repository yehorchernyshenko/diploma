using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Diploma.Models.Entities;
using Diploma.Models.ViewModels;

namespace Diploma.AutoMapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<User, AccountDetailsViewModel>();
        }
    }
}
