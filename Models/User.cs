using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace App_Server
{ 
    public class User
    {
        [BsonId]
        public ObjectId? Id { get; set; }

        [BsonElement("firstName")]
        public string? FirstName { get; set; }

        [BsonElement("lastName")]
        public string? LastName { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("password")]
        public string? Password { get; set; }

        [BsonElement("picturePath")]
        public string? PicturePath { get; set; }

        [BsonElement("friends")]
        public List<string>? Friends { get; set; }

        [BsonElement("location")]
        public string? Location { get; set; }

        [BsonElement("occupation")]
        public string? Occupation { get; set; }

        [BsonElement("viewedProfile")]
        public int ViewedProfile { get; set; }

        [BsonElement("impressions")]
        public int Impressions { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("__v")]
        public int V { get; set; }
    }
}
