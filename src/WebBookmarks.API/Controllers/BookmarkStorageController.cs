using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using WebBookmarks.Domain;
using WebBookmarks.GrainInterfaces;

namespace WebBookmarks.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookmarkStorageController : ControllerBase
    {
        private readonly ILogger<BookmarkStorageController> _logger;
        private readonly IClusterClient _client;

        public BookmarkStorageController(ILogger<BookmarkStorageController> logger, IClusterClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet("{userId:Guid:required}")]
        public async Task<ActionResult<BookmarkStorageState>> GetBookmarkStorageByUserId(Guid userId)
        {
            var grain = _client.GetGrain<IBookmarkStorageGrain>(userId);
            return Ok(await grain.GetBookmarkStorage());
        }

        [HttpPost("{userId:Guid:required}")]
        public async Task<IActionResult> UpdateBookmarkStorage(Guid userId, [FromBody] BookmarkStorageState bookmarkStorage)
        {
            var grain = _client.GetGrain<IBookmarkStorageGrain>(userId);
            await grain.UpdateBookmarkStorage(bookmarkStorage);
            return Ok();
        }
    }
}
