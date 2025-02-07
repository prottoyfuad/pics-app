
using MongoDB.Driver;
using WebAppController.Interfaces;
using WebAppController.Models;

namespace WebAppController.DataLoaders
{
    public class MongoDataLoader : IDataLoader
    {
        private readonly IConfiguration config;
        private readonly IMongoDatabase database;

        private readonly IMongoCollection<User> users;
        private readonly IMongoCollection<Image> images;

        public MongoDataLoader(IConfiguration _config)
        {
            Console.WriteLine("@Mongo");
            config = _config;
            var conString = config["MongoDB:ConnectionString"];
            if (conString == null)
            {
                throw new ArgumentNullException("MongoDB Connection String not found.");
            }
            var mongoUrl = MongoUrl.Create(conString);
            if (mongoUrl == null)
            {
                throw new ArgumentNullException("MongoDB Url not created.");
            }
            var mongoClient = new MongoClient(mongoUrl);
            if (mongoClient == null)
            {
                throw new ArgumentNullException("MongoDB Client not created.");
            }
            database = mongoClient.GetDatabase(mongoUrl.DatabaseName);

            users = database.GetCollection<User>("users");
            images = database.GetCollection<Image>("images");

            if (users == null || images == null)
            {
                throw new ArgumentNullException("user or image null");
            }
            Console.WriteLine($"Done, dbName : {mongoUrl.DatabaseName}");
        }

        public async Task<bool> AddUserAsync(User user)
        {
            User? prevUser = await GetUserAsync(user.UserName);
            if (prevUser == null)
            {
                await users.InsertOneAsync(user);
                return true;
            }
            return false;
        }

        public async Task<User?> GetUserAsync(string username)
        {
            var filter = Builders<User>.Filter.Eq(x => x.UserName, username);
            var user = users.Find(filter).FirstOrDefault();
            return await Task.FromResult(user);
        }

        public async Task<bool> AddImageAsync(Image image)
        {
            throw new NotImplementedException();
        }

        public async Task<Image?> GetImageAsync(int Id)
        {
            var filter = Builders<Image>.Filter.Empty;
            Image? image = null;
            if (images.CountDocuments(filter) > Id) {
                var sortOptions = Builders<Image>.Sort.Descending(image => image.Id);
                image = images.Find(filter)
                              //.Sort(sortOptions)
                              .Skip(Id)
                              .Limit(1)
                              .ToList()[0];
            }
            return await Task.FromResult(image);
        }
    }
}
