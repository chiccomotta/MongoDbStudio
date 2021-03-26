using System;
using System.Collections.Generic;
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
                Name = "Gianni Motta",
                Email = "gianni.motta@hotmail.it",
                Hobbies = new List<string>(){"Calcio", "Pianoforte", "Pasticceria", "Viaggi"}
            };

            var db = MongoDbDbClient.GetDatabase("MongoDbStudio");
            var users = db.GetCollection<User>("Users");
            await users.InsertOneAsync(person);

            return await Task.FromResult(Ok(person));
        }

        [Route("UpdateMongoUser")]
        [HttpGet]
        public async Task<OkObjectResult> UpdateMongoUser()
        {
            var db = MongoDbDbClient.GetDatabase("MongoDbStudio");
            var users = db.GetCollection<User>("Users");

            // Filter
            var filter = Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse("605da17e3a0e620b39d8169c"));
            
            // Update
            var update = Builders<User>
                .Update
                .Set(s => s.Name, "Randazzo")
                .Set(u => u.Email, "randazzo@hotmail.it");

            var result = await users.UpdateOneAsync(filter, update);

            return await Task.FromResult(Ok(result));
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
