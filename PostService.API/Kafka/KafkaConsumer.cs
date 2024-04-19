using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PostService.API.Models;
using PostService.API.Services;
using System.Text.Json;

namespace PostService.API.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> _log;
        private readonly IConsumer<Null, string> _consumer;
        private readonly IPostService _service;

        public KafkaConsumer(ILogger<KafkaConsumer> log, IConsumer<Null, string> consumer, IPostService service)
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
                var consumeResult = _consumer.Consume(stoppingToken);
                var mv = consumeResult.Message.Value;
                _log.LogInformation(mv);

                try
                {
                    var t = JsonSerializer.Deserialize<ThreadIdName>(mv);
                    var p = t != null ? await _service.GetPostsByThreadId(t.Id) : null;
                    foreach (var p2 in p)
                    {
                        p2.ThreadName = t?.Name;
                        await _service.UpdatePost(p2);
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
        }

        public override void Dispose()
        {
            _consumer.Dispose();
            base.Dispose();
        }
        private class ThreadIdName
        {
            public string Id { get; set; } = null!;
            public string Name { get; set; } = null!;
        }
    }
}
