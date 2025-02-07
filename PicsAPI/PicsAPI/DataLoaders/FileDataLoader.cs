
using System.Text.Json;
using WebAppController.Interfaces;
using WebAppController.Models;

namespace WebAppController.DataLoaders {
  public class FileDataLoader : IDataLoader
    {
        private readonly string userListPath = Path.Combine(Directory.GetCurrentDirectory(), "Data/userList.json");
        private readonly string imageListPath = Path.Combine(Directory.GetCurrentDirectory(), "Data/imageList.json");

        private bool wasInitialized = false;
        private List<Image> images = [];
        private List<User> users = [];

        public FileDataLoader()
        {
            initialize();
        }

        public Task<User?> GetUserAsync(string username)
        {
            var user = users.FirstOrDefault(usr => usr.UserName == username);
            return Task.FromResult(user);
        }

        public Task<Image?> GetImageAsync(int Id)
        {
            Image? image = null;
            if (Id >= 0 || Id < images.Count)
            {
                image = images[Id];
            }
            return Task.FromResult(image);
        }

        public async Task<bool> AddUserAsync(User usr)
        {
            var prevUser = await GetUserAsync(usr.UserName);
            if (prevUser == null)
            {
                users.Add(usr);
                await WriteToJsonAsync(userListPath, users);
                return true;
            }
            return false;
        }

        public Task<bool> AddImageAsync(Image image)
        {
            throw new NotImplementedException();
        }

        private async void initialize()
        {
            if (wasInitialized)
            {
                return;
            }
            users = await ReadFromJsonAsync<User>(userListPath);
            images = await ReadFromJsonAsync<Image>(imageListPath);
            wasInitialized = true;
        }

        private static List<Image> ReadImagesExplicitly()
        {
            string _imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data/Images");
            List<string> imageDirs = Directory.GetFiles(_imagesDirectory)
                                 .Where(f =>
                                     (new[] { ".jpg" }).Contains(Path.GetExtension(f).ToLower())
                                 ).ToList();

            List<Image> res = [];
            for (int i = 0; i < imageDirs.Count; i++)
            {
                var fileBytes = File.ReadAllBytes(imageDirs[i]);
                res.Add(new Image
                {
                    FileName = Path.GetFileName(imageDirs[i]),
                    FileData = Convert.ToBase64String(fileBytes)
                });
            }
            return res;
        }

        public static async Task WriteToJsonAsync<T>(string path, List<T> list)
        {
            string json = JsonSerializer.Serialize(list,
                new JsonSerializerOptions { WriteIndented = true }
            );
            await File.WriteAllTextAsync(path, json);
        }
        public static async Task<List<T>> ReadFromJsonAsync<T>(string path)
        {
            if (!File.Exists(path))
            {
                return new List<T>();
            }
            string json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }
}
