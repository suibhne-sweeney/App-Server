using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace App_Server
{
    public class Post
    {
        [BsonId]
        public ObjectId Id {get; set;}

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        [BsonElement("firstName")]
        public string? FirstName { get; set; }

        [BsonElement("lastName")]
        public string? LastName { get; set; }

        [BsonElement("location")]
        public string? Location { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("picturePath")]
        public string? PicturePath { get; set; }

        [BsonElement("userPicturePath")]
        public string? UserPicturePath { get; set; }

        [BsonElement("likes")]
        public Dictionary<string, bool>? Likes { get; set; }

        [BsonElement("comments")]
        public List<string>? Comments { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("__v")]
        public int V {get; set;}
    }
}