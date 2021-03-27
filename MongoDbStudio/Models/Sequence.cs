using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbStudio.Models
{
    public class Sequence
    {
        [BsonId] 
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}