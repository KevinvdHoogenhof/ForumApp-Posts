using Mongo2Go;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
