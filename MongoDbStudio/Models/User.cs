using MongoDB.Bson;

namespace MongoDbStudio.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
