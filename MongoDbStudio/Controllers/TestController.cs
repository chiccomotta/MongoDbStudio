using System;
using Microsoft.AspNetCore.Mvc;
using MongoDbStudio.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbStudio.Models;

namespace MongoDbStudio.Controllers
{
    [Route("Test")]
    [ApiController]
    public class TestController : ApiControllerBase
    {
        private readonly IMongoClient MongoDbDbClient;
        //private readonly IMongoCollection<User> UsersCollection;

        public TestController(IInfrastructureService infrastructure, IMongoClient mongoDbClient) : base(infrastructure)
        {
            MongoDbDbClient = mongoDbClient;
        }

        [Route("CreateMongoUser")]
        [HttpGet]
        public async Task<OkObjectResult> CreateMongoUser()
        {
            var person = new User
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Cristiano",
                Email = "cristiano.motta@hotmail.it"
            };

            var db = MongoDbDbClient.GetDatabase("MongoDbStudio");
            var users = db.GetCollection<User>("Users");
            await users.InsertOneAsync(person);

            return await Task.FromResult(Ok(person));
        }

        [Route("GetUsers")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var filter = new BsonDocument();
            var db = MongoDbDbClient.GetDatabase("MongoDbStudio");

            var users = db.GetCollection<User>("Users");
            var query = await users.FindAsync(filter);

            return Ok(await query.ToListAsync());
        }
    }
}
