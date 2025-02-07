
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAppController.Models {
    public class Image {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {
            get; set;
        }

        [BsonElement("FileName")]
        [BsonRequired]
        public required string FileName {
            get; set;
        }

        [BsonElement("FileData")]
        [BsonRequired]
        public required string FileData {
            get; set;
        }
    };
}
