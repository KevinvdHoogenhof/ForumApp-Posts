using Mongo2Go;
using MongoDB.Driver;

namespace PostService.Tests
{
    public class MongoDbFixture : IDisposable
    {
        public MongoDbRunner Runner { get; set; }
        public MongoClient Client { get; set; }

        public MongoDbFixture()
        {
            Runner = MongoDbRunner.Start();
            Client = new MongoClient(Runner.ConnectionString);
        }

        public void Dispose()
        {
            Runner.Dispose();
        }
    }
}
