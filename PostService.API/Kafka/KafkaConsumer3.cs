using Confluent.Kafka;
using PostService.API.Services;
using System.Text.Json;

namespace PostService.API.Kafka
{
    public class KafkaConsumer3 : BackgroundService
    {
        private readonly ILogger<KafkaConsumer3> _log;
        private readonly IConsumer<Null, string> _consumer;
        private readonly IPostService _service;

        public KafkaConsumer3(ILogger<KafkaConsumer3> log, IConsumer<Null, string> consumer, IPostService service)
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
                    _consumer.Subscribe("newcomment");

                    var consumeResult = _consumer.Consume(stoppingToken);
                    var mv = consumeResult.Message.Value;
                    _log.LogInformation(mv);

                    try
                    {
                        var t = JsonSerializer.Deserialize<PostIdComments>(mv);
                        var p = t != null ? await _service.GetPost(t.PostId) : null;
                        p.Comments = t.Comments;
                        await _service.UpdatePost(p);
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
        private class PostIdComments
        {
            public string PostId { get; set; } = null!;
            public int Comments { get; set; } = 0;
        }
    }
}
