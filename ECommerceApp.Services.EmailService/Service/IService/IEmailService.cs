using ECommerceApp.Services.EmailService.Models;
using ECommerceApp.Services.EmailService.Models.DTO;

namespace ECommerceApp.Services.EmailService.Service.IService;

public interface IEmailService
{
    Task EmailCartAndLog(CartDto cartDto);
    Task RegisterUserEmailAndLog(string email);
    Task LogOrderPlaced(RewardsMessage rewardsDto);
}
