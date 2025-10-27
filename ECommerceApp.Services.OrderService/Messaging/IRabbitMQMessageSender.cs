namespace ECommerceApp.Services.OrderService.Messaging;

public interface IRabbitMQMessageSender
{
	Task SendMessage(object message, string queueName);
}
