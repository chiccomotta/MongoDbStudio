using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MongoDbStudio.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [BsonElement("address")]
        public string FullAddress { get; set; }
        public List<string> Hobbies { get; set; }
    }
}
