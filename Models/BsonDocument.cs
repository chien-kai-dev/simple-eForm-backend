using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class BsonDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; } = null!;
    public string title { get; set; } = null!;
    public string author { get; set; } = null!;
}
