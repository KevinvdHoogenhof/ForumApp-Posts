using Confluent.Kafka;

namespace PostService.API.Kafka
{
    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;

        public KafkaProducer(IProducer<Null, string> producer, string topic)
        {
            _producer = producer;
            _topic = topic;
        }

        public async Task Produce(string m, CancellationToken cancellationToken)
        {
            await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = m }, cancellationToken);
        }

        public async Task ProduceMultiple(IReadOnlyCollection<string> messages, CancellationToken cancellationToken)
        {
            await Task.WhenAll(messages.Select(t => _producer.ProduceAsync(_topic, new Message<Null, string> { Value = t }, cancellationToken)));
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
