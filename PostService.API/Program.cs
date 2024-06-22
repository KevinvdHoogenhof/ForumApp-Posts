using Confluent.Kafka;
using MongoDB.Driver;
using PostService.API.Context;
using PostService.API.Kafka;
using PostService.API.SeedData;
using PostService.API.Services;
using Prometheus;

namespace PostService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMetrics();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Database
            var connString = builder.Configuration.GetConnectionString("MongoDB");
            builder.Services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(connString));

            builder.Services.AddSingleton<IDataSeedingConfiguration, DataSeedingConfiguration>();

            builder.Services.AddSingleton<IPostContext, PostContext>();

            builder.Services.AddSingleton<IPostService, Services.PostService>();

            //Kafka producer
            var producerConfig = builder.Configuration.GetSection("ProducerConfig").Get<ProducerConfig>();
            var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            builder.Services.AddSingleton<IKafkaProducer>(_ => new KafkaProducer(producer, "newpost"));

            var producerConfig2 = builder.Configuration.GetSection("ProducerConfig").Get<ProducerConfig>();
            var producer2 = new ProducerBuilder<Null, string>(producerConfig2).Build();
            builder.Services.AddSingleton<IKafkaProducer2>(_ => new KafkaProducer2(producer2, "updatepostname"));

            //Kafka consumer
            var consumerConfig = builder.Configuration.GetSection("ConsumerConfig").Get<ConsumerConfig>();
            var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            //consumer.Subscribe("updatethreadname");

            //Kafka consumer 2
            var consumer2 = new ConsumerBuilder<Null, string>(consumerConfig).Build();

            //Kafka consumer 3
            var consumer3 = new ConsumerBuilder<Null, string>(consumerConfig).Build();

            builder.Services.AddHostedService(sp =>
                new KafkaConsumer(sp.GetRequiredService<ILogger<KafkaConsumer>>(), consumer, sp.GetRequiredService<IPostService>()));

            builder.Services.AddHostedService(sp =>
                new KafkaConsumer2(sp.GetRequiredService<ILogger<KafkaConsumer2>>(), consumer2, sp.GetRequiredService<IPostService>()));

            builder.Services.AddHostedService(sp =>
                new KafkaConsumer3(sp.GetRequiredService<ILogger<KafkaConsumer3>>(), consumer3, sp.GetRequiredService<IPostService>()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseHttpMetrics();
            app.UseMetricServer();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
