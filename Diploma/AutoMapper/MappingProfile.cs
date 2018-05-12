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
