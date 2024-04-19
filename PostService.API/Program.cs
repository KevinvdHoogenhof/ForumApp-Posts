
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PostService.API.Context;
using PostService.API.Models;
using PostService.API.SeedData;
using PostService.API.Services;

namespace PostService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

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

            //Kafka consumer
            var consumerConfig = builder.Configuration.GetSection("ConsumerConfig").Get<ConsumerConfig>();
            var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            consumer.Subscribe("updatethreadname");

            builder.Services.AddHostedService(sp =>
                new KafkaConsumer(sp.GetRequiredService<ILogger<KafkaConsumer>>(), consumer, sp.GetRequiredService<IPostService>()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
