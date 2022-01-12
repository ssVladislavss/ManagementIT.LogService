using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logs.Core.Abstractions;
using Logs.Core.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.Extensions.Configuration;

namespace Logs.DataAccess.Data
{
    public class LogService : IMongoService
    {
        IGridFSBucket gridFS;
        IMongoCollection<LogMessageEntity> logCollection;
        private readonly IConfiguration _configuration;

        public LogService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            string connectionString = _configuration.GetSection("ConnectionStrings").GetSection("Mongo").Value;
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase(connection.DatabaseName);
            gridFS = new GridFSBucket(database);
            logCollection = database.GetCollection<LogMessageEntity>(_configuration.GetSection("ConnectionStrings").GetSection("MongoCollection").Value);

        }
        public async Task Create(LogMessageEntity log) => await logCollection.InsertOneAsync(log);
        
        public async Task<LogMessageEntity> GetAsync(string id) => 
            await logCollection.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        
        public async Task<IEnumerable<LogMessageEntity>> GetLogsAsync(string? type, string? iniciator)
        {
            var builder = new FilterDefinitionBuilder<LogMessageEntity>();
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(type)) filter = filter & builder.Regex("Type", new BsonRegularExpression(type));
            if (!string.IsNullOrEmpty(iniciator)) filter = filter & builder.Regex("Iniciator", new BsonRegularExpression(iniciator));
            
            return await logCollection.Find(filter).ToListAsync();
        }

        public async Task DeleteRangeAsync()
        {
            var builder = new FilterDefinitionBuilder<LogMessageEntity>();
            var filter = builder.Empty;
            await logCollection.DeleteManyAsync(filter);
        }

        public async Task<bool> DeleteSelectedAsync(List<string> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    await logCollection.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                await logCollection.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}