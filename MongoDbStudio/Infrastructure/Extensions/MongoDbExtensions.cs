using MongoDB.Driver;
using MongoDbStudio.Models;
using System.Threading.Tasks;

namespace MongoDbStudio.Infrastructure.Extensions
{
    public static class MongoDbExtensions
    {
        public static async Task<Sequence> GetNextSequenceValue(this IMongoDatabase db, string sequenceName)
        {
            // Prendo la collection delle sequences
            var sequences = db.GetCollection<Sequence>("Sequences");
            
            // Prendo la sequence selezionata
            var filter = Builders<Sequence>.Filter.Eq(a => a.Name, sequenceName);
            var update = Builders<Sequence>.Update.Inc(a => a.Value, 1);

            // Leggo e incremento il contatore in maniera transazionale
            var sequence = await sequences.FindOneAndUpdateAsync(filter, update, 
                new FindOneAndUpdateOptions<Sequence>()
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After,
                });            

            return sequence;
        }
    }
}
