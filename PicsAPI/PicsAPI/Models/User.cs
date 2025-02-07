
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAppController.Models {

    public class User {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {
            get; set;
        }

        [BsonElement("username")]
        [BsonRequired]
        public required string UserName { get; set;
        }

        [BsonElement("pass_hash")]
        [BsonRequired]
        public required byte[] PasswordHash { get; set;
        }

        [BsonElement("pass_salt")]
        [BsonRequired]
        public required byte[] PasswordSalt { get; set; }
    }
}
