using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbStudio.Interfaces;
using MongoDbStudio.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDbStudio.Infrastructure.Extensions;

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

        [Route("create")]
        [HttpGet]
        public async Task<OkObjectResult> CreateMongoUser()
        {
            var db = MongoDbDbClient.GetDatabase("MongoDbStudio");
            var users = db.GetCollection<User>("Users");
            
            var person = new User
            {
                Id = db.GetNextSequenceValue("UserSequence").Result.Value,
                Name = "Mauro",
                Email = "mauro.motta@hotmail.it",
                Hobbies = new List<string>(){"Calcio", "Pianoforte", "Pasticceria", "Viaggi"}
            };

            await users.InsertOneAsync(person);
            return await Task.FromResult(Ok(person));
        }

        [Route("update")]
        [HttpGet]
        public async Task<OkObjectResult> UpdateMongoUser()
        {
            var db = MongoDbDbClient.GetDatabase("MongoDbStudio");
            var users = db.GetCollection<User>("Users");

            // Filter
            var filter = Builders<User>.Filter.Eq(u => u.Name, "Chicco");
            
            // Update
            var update = Builders<User>
                .Update
                .Set(s => s.Name, "Randazzo")
                .Set(u => u.Email, "randazzo@hotmail.it")
                .Push(u => u.Hobbies, "new value" + DateTime.Now.Second);

            var result = await users.UpdateOneAsync(filter, update);

            return await Task.FromResult(Ok(result));
        }

        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var db = MongoDbDbClient.GetDatabase("MongoDbStudio");

            var users = db.GetCollection<User>("Users");
            var query = users.Find(u => u.Name == "Chicco");

            return Ok(await query.ToListAsync());
        }
    }
}
