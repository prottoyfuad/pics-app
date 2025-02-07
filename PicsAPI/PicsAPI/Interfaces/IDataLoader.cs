
using WebAppController.Models;

namespace WebAppController.Interfaces {
  public interface IDataLoader {
        public Task<User?> GetUserAsync(string username);
        public Task<Image?> GetImageAsync(int Id);
        public Task<bool> AddUserAsync(User usr);
        public Task<bool> AddImageAsync(Image image);
    }
}
