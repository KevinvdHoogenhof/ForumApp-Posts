using Confluent.Kafka;
using PostService.API.Services;
using System.Text.Json;

namespace PostService.API.Kafka
{
    public class KafkaConsumer2 : BackgroundService
    {
        private readonly ILogger<KafkaConsumer2> _log;
        private readonly IConsumer<Null, string> _consumer;
        private readonly IPostService _service;

        public KafkaConsumer2(ILogger<KafkaConsumer2> log, IConsumer<Null, string> consumer, IPostService service)
        {
            _log = log;
            _consumer = consumer;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _consumer.Subscribe("accountdeleted");

                    var consumeResult = _consumer.Consume(stoppingToken);
                    var mv = consumeResult.Message.Value;
                    _log.LogInformation(mv);

                    try
                    {
                        var accountId = 0;
                        var parts = mv.Split(" ");
                        if (parts.Length > 0)
                        {
                            accountId = JsonSerializer.Deserialize<int>(parts[0]);
                            //_log.LogInformation(accountId.ToString());
                            var posts = accountId != 0 ? await _service.GetPostsByAuthorId(accountId) : null;
                            foreach (var post in posts)
                            {
                                await _service.DeletePost(post.Id);
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                    }

                    if (i++ % 1000 == 0)
                    {
                        _consumer.Commit();
                    }
                }
                catch (ConsumeException ex)
                {
                    _log.LogInformation($"Error consuming message: {ex.Error.Reason}");

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "An unexpected error occurred while consuming messages.");
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
