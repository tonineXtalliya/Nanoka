using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nanoka.Models;
using Nanoka.Models.Requests;
using Nanoka.Storage;

namespace Nanoka.Controllers
{
    [ApiController]
    [Route("books")]
    [Authorize]
    public class BookController : ControllerBase
    {
        readonly BookManager _bookManager;
        readonly IStorage _storage;

        public BookController(BookManager bookManager, IStorage storage)
        {
            _bookManager = bookManager;
            _storage     = storage;
        }

        [HttpGet("{id}")]
        public async Task<Book> GetAsync(string id)
            => await _bookManager.GetAsync(id);

        [HttpPut("{id}")]
        [UserClaims(unrestricted: true)]
        public async Task<Book> UpdateAsync(string id, BookBase book)
            => await _bookManager.UpdateAsync(id, book);

        [HttpDelete("{id}")]
        [UserClaims(unrestricted: true, reputation: 100, reason: true)]
        [VerifyHuman]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            await _bookManager.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("{id}/snapshots")]
        public async Task<Snapshot<Book>[]> GetSnapshotsAsync(string id)
            => await _bookManager.GetSnapshotsAsync(id);

        [HttpPost("{id}/snapshots/revert")]
        [UserClaims(unrestricted: true, reason: true)]
        public async Task<Book> RevertAsync(string id, RevertEntityRequest request)
            => await _bookManager.RevertAsync(id, request.SnapshotId);

        [HttpPut("{id}/vote")]
        public async Task<Vote> SetVoteAsync(string id, VoteBase vote)
            => await _bookManager.VoteAsync(id, vote.Type);

        [HttpDelete("{id}/vote")]
        public async Task<ActionResult> UnsetVoteAsync(string id)
        {
            await _bookManager.VoteAsync(id, null);
            return Ok();
        }

        [HttpGet("{id}/contents/{contentId}")]
        public async Task<BookContent> GetContentAsync(string id, string contentId)
            => (await _bookManager.GetContentAsync(id, contentId)).content;

        [HttpPut("{id}/contents/{contentId}")]
        public async Task<BookContent> UpdateContentAsync(string id, string contentId, BookContentBase content)
            => await _bookManager.UpdateContentAsync(id, contentId, content);

        [HttpDelete("{id}/contents/{contentId}")]
        [UserClaims(unrestricted: true, reputation: 100, reason: true)]
        [VerifyHuman]
        public async Task<ActionResult> DeleteContentAsync(string id, string contentId)
        {
            await _bookManager.RemoveContentAsync(id, contentId);
            return Ok();
        }

        [HttpGet("{id}/contents/{contentId}/images/{index}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetImageAsync(string id, string contentId, int index)
        {
            var file = await _storage.ReadAsync($"{id}/{contentId}/{index}");

            if (file == null)
                return NotFound();

            return new FileStreamResult(file.Stream, file.MediaType);
        }

        [HttpPost("search")]
        public async Task<SearchResult<Book>> SearchAsync(BookQuery query)
            => await _bookManager.SearchAsync(query);
    }
}