namespace PostService.API.Kafka
{
    public interface IKafkaProducer2
    {
        Task Produce(string message, CancellationToken cancellationToken);
        Task ProduceMultiple(IReadOnlyCollection<string> messages, CancellationToken cancellationToken);
    }
}
