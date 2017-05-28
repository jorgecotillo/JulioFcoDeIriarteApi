using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using ServicesAPI.Interfaces;

namespace ServicesAPI.Models
{
    public class NotificationModel
    {
        [Required]
        [MaxLength(45, ErrorMessage = "Title exceeded the allowed value")]
        public string Title { get; set; }
        [Required]
        [MaxLength(250, ErrorMessage = "Message exceeded the allowed value")]
        public string Message { get; set; }
    }

    public class NotificationStore : INotificationStore
    {
        readonly IMongoClient _client;
        readonly IMongoDatabase _database;
        string _collectionName = "announcements";

        public NotificationStore(IConfigurationRoot configuration)
        {
            var connString = configuration["Mongo:ConnectionString"];
            var dbName = configuration["Mongo:Database"];
            _client = new MongoClient(connString);
            _database = _client.GetDatabase(dbName);
        }
        
        public async Task SaveNotification(NotificationModel notification)
        {
            var document =
                new BsonDocument
                        {
                            { "announcement", new BsonDocument
                                {
                                    { "title", notification.Title },
                                    { "message", notification.Message }
                                }
                            },
                            { "created_date", new BsonDateTime(DateTime.UtcNow) },
                            { "announcement_id", Guid.NewGuid().ToString() }
                        };
            var collection = _database.GetCollection<BsonDocument>(_collectionName);
            await collection.InsertOneAsync(document);
        }
        
        public async Task<IEnumerable<NotificationModel>> GetAllNotifications(
            DateTime? from = null,
            int numberOfRecords = 10)
        {
            //Let's set a default date in the past
            if (!from.HasValue)
            {
                from = DateTime.Parse("01/01/1900");
            }

            var collection = _database.GetCollection<BsonDocument>(_collectionName);
            var filter = 
                Builders<BsonDocument>
                .Filter
                .Gte("created_date", new BsonDateTime(from.Value));
            var result = 
                await collection.Find(filter).Limit(numberOfRecords).ToListAsync();

            List<NotificationModel> notificationModels = new List<NotificationModel>();

            foreach (BsonDocument document in result)
            {
                notificationModels.Add(new NotificationModel
                {
                    Title = document["announcement"]["title"].AsString,
                    Message = document["announcement"]["message"].AsString
                });
            }

            return notificationModels;
        }
    }
}
