using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ECommerceApp.Services.CartService.Messaging;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
	private ConnectionFactory _factory;
	private IConnection _connection;
	private IChannel _channel;

	private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
	public RabbitMQMessageSender(IConfiguration configuration)
	{
		_factory = new ConnectionFactory{ HostName = "localhost" };
	}
	public async Task SendMessageAsync(object message, string queueName)
	{
		await _lock.WaitAsync();
		try
		{
			if (_connection == null || !_connection.IsOpen)
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
		finally
		{
			_lock.Release();
		}
	}

	private async Task InitializeRabbitMQ(string queueName)
	{
		try
		{
			_connection = await _factory.CreateConnectionAsync();
			_channel = await _connection.CreateChannelAsync();

			await _channel.QueueDeclareAsync(
				queue:queueName,
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

	public async ValueTask DisposeAsync()
	{
		try
		{
			if (_channel != null)
			{
				await _channel.CloseAsync();
				_channel.Dispose();
			}
			if (_connection != null)
			{
				await _connection.CloseAsync();
				_connection.Dispose();
			}
		}
		catch (Exception ex)
		{
			// Log exception (not implemented here)
		}
	}
}
