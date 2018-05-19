using System.Threading.Tasks;
using Diploma.Models;
using Diploma.Models.Entities;
using Diploma.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly ApplicationContext _applicationContext;

        public ApplicationService(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task LeaveMessage(LeaveMessageViewModel leaveMessageViewModel)
        {
            var userMessage = new UserMessage
            {
                Message = leaveMessageViewModel.Message,
                Email = leaveMessageViewModel.Email,
                PhoneNumber = leaveMessageViewModel.PhoneNumber,
                UserId = leaveMessageViewModel.UserId
            };

            _applicationContext.UserMessages.Add(userMessage);
            await _applicationContext.SaveChangesAsync();
        }
    }

    public interface IApplicationService
    {
        Task LeaveMessage(LeaveMessageViewModel leaveMessageViewModel);
    }
}