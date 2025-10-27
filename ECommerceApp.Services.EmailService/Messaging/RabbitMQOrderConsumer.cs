
using ECommerceApp.Services.EmailService.Models.DTO;
using ECommerceApp.Services.EmailService.Service.IService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ECommerceApp.Services.EmailService.Messaging;

public class RabbitMQOrderConsumer : BackgroundService
{
	private readonly IConfiguration _configuration;
	private readonly IServiceScopeFactory _scopeFactory;

	private readonly string _queueName;
	private readonly IConnectionFactory _factory;
	private IConnection _connection;
	private IChannel _channel;

	public RabbitMQOrderConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory)
	{
		_configuration = configuration;
		_scopeFactory = scopeFactory;

		_factory = new ConnectionFactory
		{
			HostName = "localhost"
		};	
		_queueName = _configuration.GetValue<string>("TopicAndQueueNames:OrderQueue");
	}
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{

			try
			{
				await InitializeRabbitMQAsync(_queueName);

				var consumer = new AsyncEventingBasicConsumer(_channel);
				consumer.ReceivedAsync += async (model, ea) =>
				{
					try
					{
						var body = ea.Body.ToArray();
						var message = Encoding.UTF8.GetString(body);

						var orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(message);
						using (var scope = _scopeFactory.CreateScope())
						{
							var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
							await emailService.EmailOrderPlacedAndLog(orderHeaderDto);
						}

						// Acknowledge message manually
						await _channel.BasicAckAsync(ea.DeliveryTag, false);
					}
					catch (Exception ex)
					{
						//log
					}
				};

				await _channel.BasicConsumeAsync(
					queue: _queueName,
					autoAck: false,
					consumer: consumer);
				await Task.Delay(Timeout.Infinite, stoppingToken);
			}
			catch(OperationCanceledException)
			{
				break; // Graceful shutdown
			}
			catch (Exception ex)
			{
				//log
				await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
			}
		}
		await DisposeConnectionAsyc();
	}

	private async Task InitializeRabbitMQAsync(string queueName)
	{
		if(_connection != null && _connection.IsOpen)
		{
			return ;
		}
		_connection = await _factory.CreateConnectionAsync();
		_channel = await _connection.CreateChannelAsync();

		await _channel.QueueDeclareAsync(
			queue: queueName,
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null);
	}

	private async Task DisposeConnectionAsyc()
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
				await _connection.DisposeAsync();
			}
		}
		catch { }
	}

	public override async Task StopAsync(CancellationToken cancellationToken)
	{
		await DisposeConnectionAsyc();
		await base.StopAsync(cancellationToken);
	}
}
