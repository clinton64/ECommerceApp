using ECommerceApp.Services.EmailService.Models.DTO;
using ECommerceApp.Services.EmailService.Service;
using ECommerceApp.Services.EmailService.Service.IService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMQCartConsumer : BackgroundService
{
	private readonly IConfiguration _configuration;
	private readonly IServiceScopeFactory _scopeFactory;

	private IConnection _connection;
	private IChannel _channel;
	private ConnectionFactory _factory;
	private string _queueName;

	public RabbitMQCartConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory)
	{
		_configuration = configuration;
		_scopeFactory = scopeFactory;

		_factory = new ConnectionFactory
		{
			HostName = "localhost"
		};

		_queueName = _configuration.GetValue<string>("TopicAndQueueNames:CartQueue");
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await InitializeRabbitMQAsync();

				var consumer = new AsyncEventingBasicConsumer(_channel);
				consumer.ReceivedAsync += async (model, ea) =>
				{
					try
					{
						var body = ea.Body.ToArray();
						var message = Encoding.UTF8.GetString(body);

						var cartDto = JsonConvert.DeserializeObject<CartDto>(message);

						using (var scope = _scopeFactory.CreateScope())
						{
							var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
							await emailService.EmailCartAndLog(cartDto);
						}

						// Acknowledge message manually
						await _channel.BasicAckAsync(ea.DeliveryTag, false);
					}
					catch (Exception ex)
					{
						// Optionally: Nack the message to requeue
						// await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
					}
				};

				// autoAck false to ensure reliability
				await _channel.BasicConsumeAsync(
					queue: _queueName,
					autoAck: false,
					consumer: consumer);

				await Task.Delay(Timeout.Infinite, stoppingToken);
			}
			catch (OperationCanceledException)
			{
				break; // Graceful shutdown
			}
			catch (Exception ex)
			{
				// log
				await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
			}
		}

		await DisposeConnectionAsync();
	}

	private async Task InitializeRabbitMQAsync()
	{
		if (_connection != null && _connection.IsOpen)
			return;

		_connection = await _factory.CreateConnectionAsync();
		_channel = await _connection.CreateChannelAsync();

		await _channel.QueueDeclareAsync(
			queue: _queueName,
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null);

	}

	private async Task DisposeConnectionAsync()
	{
		try
		{
			if (_channel != null)
			{
				await _channel.CloseAsync();
				await _channel.DisposeAsync();
			}

			if (_connection != null)
			{
				await _connection.CloseAsync();
				_connection.Dispose();
			}
		}
		catch { }
	}

	public override async Task StopAsync(CancellationToken cancellationToken)
	{
		await DisposeConnectionAsync();
		await base.StopAsync(cancellationToken);
	}
}
