using ECommerceApp.Services.EmailService.Data;
using ECommerceApp.Services.EmailService.Models;
using ECommerceApp.Services.EmailService.Models.DTO;
using ECommerceApp.Services.EmailService.Service.IService;
using System.Text;

namespace ECommerceApp.Services.EmailService.Service;

public class EmailService : IEmailService
{
    private readonly AppDbContext _dbContext;

    public EmailService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EmailCartAndLog(CartDto cartDto)
    {
        StringBuilder message = new StringBuilder();

        message.AppendLine("<br/>Cart Email Requested ");
        message.AppendLine("<br/>Total " + cartDto.CartHeader.CartTotal);
        message.Append("<br/>");
        message.Append("<ul>");
        foreach (var item in cartDto.CartDetails)
        {
            message.Append("<li>");
            message.Append(item.ProductId + " x " + item.Count);
            message.Append("</li>");
        }
        message.Append("</ul>");

        await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
    }

    public async Task EmailOrderPlacedAndLog(OrderHeaderDto order)
    {
	    string message = "New Order Placed. <br/> Order ID : " + order.Id;
	    await LogAndEmail(message, order.Email ?? "chakmaclinton86@gmail.com");
    }

    public async Task RegisterUserEmailAndLog(string email)
    {
        string message = "User Registeration Successful. <br/> Email : " + email;
        await LogAndEmail(message, "chakmaclinton86@gmail.com");
    }

    private async Task<bool> LogAndEmail(string message, string email)
    {
        try
        {
            EmailLogger emailLog = new()
            {
                Email = email,
                EmailSent = DateTime.Now,
                Message = message
            };
            await _dbContext.EmailLoggers.AddAsync(emailLog);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
