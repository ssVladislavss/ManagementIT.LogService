using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Logs.Core.Domain
{
    public class LogMessageEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Address { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string DateOrTime { get; set; }
        public string Iniciator { get; set; }

        public LogMessageEntity() { }
        public LogMessageEntity(string address, string message, string type, string iniciator)
        {
            Address = address;
            Message = message;
            Type = type;
            DateOrTime = DateTime.Now.ToString("F");
            Iniciator = iniciator;
        }
        public static LogMessageEntity GetLogMessage(string address, string message, string iniciator = "", string type = "Error") =>
            new LogMessageEntity(address, message, type, iniciator);
    }
}