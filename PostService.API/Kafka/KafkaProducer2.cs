using Confluent.Kafka;

namespace PostService.API.Kafka
{
    public class KafkaProducer2 : IKafkaProducer2, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;

        public KafkaProducer2(IProducer<Null, string> producer, string topic)
        {
            _producer = producer;
            _topic = topic;
        }

        public Task Produce(string message, CancellationToken cancellationToken) =>
            _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message }, cancellationToken);

        public Task ProduceMultiple(IReadOnlyCollection<string> messages, CancellationToken cancellationToken) =>
            Task.WhenAll(messages.Select(message => _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message }, cancellationToken)));

        public void Dispose() => _producer.Dispose();
    }
}
