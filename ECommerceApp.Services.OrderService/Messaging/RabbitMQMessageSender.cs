using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ECommerceApp.Services.OrderService.Messaging;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
	private ConnectionFactory _factory;
	private IConnection _connection;
	private IChannel _channel;

	public RabbitMQMessageSender()
	{
		_factory = new ConnectionFactory() { HostName = "localhost" };
	}
	public async Task SendMessage(object message, string queueName)
	{
		
		try
		{
			if(_connection == null || !_connection.IsOpen)
				await InitializeRabbitMQ(queueName);

			var json = JsonConvert.SerializeObject(message);
			var body = Encoding.UTF8.GetBytes(json);

			await _channel.BasicPublishAsync(
				exchange: string.Empty,
				routingKey: queueName,
				body: body);
		}
		catch (Exception ex)
		{
			// Log exception (not implemented here)
			throw;
		}	
	}

	private async Task InitializeRabbitMQ(string queueName)
	{
		try
		{
			_connection = await _factory.CreateConnectionAsync();
			_channel = await _connection.CreateChannelAsync();

			await _channel.QueueDeclareAsync(
				queue: queueName,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null);
		}
		catch (Exception ex)
		{
			// Log exception (not implemented here)
			throw;
		}	
	}
}
