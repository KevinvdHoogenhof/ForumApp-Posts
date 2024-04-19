using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using PostService.API;
using PostService.API.Context;
using PostService.API.SeedData;
using PostService.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Tests
{
    public class PostServiceTests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly HttpClient _client;
        private readonly IPostService _service;
        public PostServiceTests(MongoDbFixture fixture)
        {
            _fixture = fixture;
            var dataSeedingConfig = new DataSeedingConfiguration { SeedDataEnabled = false };
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<IMongoClient>();
                        services.AddSingleton<IMongoClient>(
                            (_) => _fixture.Client);
                    });
                });
            _client = appFactory.CreateClient();
            _service = new API.Services.PostService(new PostContext(_fixture.Client, dataSeedingConfig));
        }

        public void Dispose()
        {
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            collection.DeleteManyAsync(_ => true).Wait();
        }
    }
}
