
using Microsoft.AspNetCore.Mvc;
using WebAppController.Models;
using Microsoft.AspNetCore.Authorization;
using WebAppController.Interfaces;

namespace WebAppController.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class HomeController : Controller {
        private IDataLoader data;
        public HomeController(IDataLoader dataLoader) {
            this.data = dataLoader;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<Image>>> Index(int offset, int limit) {
            List<Image> images = new List<Image>();
            for (int i = 0; i < limit; i++) {
                var img = await data.GetImageAsync(offset + i);
                if (img == null) {
                    break;
                }
                images.Add(img);
            }
            return Ok(images);
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<Image>> Details(int id) {
            var img = await data.GetImageAsync(id);
            if (img == null) {
                return BadRequest($"Id({id}) not found!");
            }
            return Ok(img);
        }
    }
}
