using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoDbStudio.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Hobbies { get; set; }
    }
}
