namespace ECommerceApp.Services.CartService.Messaging;

public interface IRabbitMQMessageSender
{
	Task SendMessageAsync(object message, string queueName);
}
