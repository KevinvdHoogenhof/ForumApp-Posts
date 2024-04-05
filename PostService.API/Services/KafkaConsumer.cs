using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PostService.API.Models;

namespace PostService.API.Services
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> _log;
        private readonly IConsumer<string, int> _consumer;
        //private readonly Services.PostService _service; //Test

        public KafkaConsumer(ILogger<KafkaConsumer> log, IConsumer<string, int> consumer)
        {
            _log = log;
            _consumer = consumer;
            //_service = new Services.PostService(settings);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);

                _log.LogInformation(consumeResult.Message.Key + " - " + consumeResult.Message.Value);
                
                /*Post p = new Post
                {
                    ThreadId = "te",
                    AuthorId = "te",
                    Name = consumeResult.Message.Key,
                    Content = consumeResult.Message.Value.ToString()
                };

                await _service.InsertPost(p);*/

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
    }
}
