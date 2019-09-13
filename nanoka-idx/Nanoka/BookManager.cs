using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Nanoka.Database;
using Nanoka.Models;

namespace Nanoka
{
    public class BookManager
    {
        readonly INanokaDatabase _db;
        readonly ILocker _locker;
        readonly IMapper _mapper;
        readonly SnapshotManager _snapshot;
        readonly VoteManager _vote;
        readonly SoftDeleteManager _softDeleter;

        public BookManager(INanokaDatabase db, NamedLocker locker, IMapper mapper, SnapshotManager snapshot, VoteManager vote,
                           SoftDeleteManager softDeleter)
        {
            _db          = db;
            _locker      = locker.Get<BookManager>();
            _mapper      = mapper;
            _snapshot    = snapshot;
            _vote        = vote;
            _softDeleter = softDeleter;
        }

        public async Task<Book> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var book = await _db.GetBookAsync(id, cancellationToken);

            if (book == null)
                throw Result.NotFound<Book>(id).Exception;

            return book;
        }

        public async Task<(Book book, BookContent content)> GetContentAsync(int id, int contentId, CancellationToken cancellationToken = default)
        {
            var book    = await GetAsync(id, cancellationToken);
            var content = book.Contents.FirstOrDefault(c => c.Id == contentId);

            if (content == null)
                throw Result.NotFound<BookContent>(id, contentId).Exception;

            return (book, content);
        }

        public Task<Snapshot<Book>[]> GetSnapshotsAsync(int id, CancellationToken cancellationToken = default)
            => _snapshot.GetAsync<Book>(id, cancellationToken);

        public async Task<Book> RevertAsync(int id, int rollbackId, CancellationToken cancellationToken = default)
        {
            using (await _locker.EnterAsync(id, cancellationToken))
            {
                var rollback = await _snapshot.GetAsync<Book>(rollbackId, id, cancellationToken);
                var book     = await _db.GetBookAsync(id, cancellationToken);

                if (book != null && rollback.Value == null)
                {
                    await _vote.DeleteAsync(book, cancellationToken);
                    await _db.DeleteBookAsync(book, cancellationToken);

                    foreach (var content in book.Contents)
                        await _softDeleter.DeleteAsync(EnumerateBookFiles(book, content), cancellationToken);

                    book = null;
                }
                else if (rollback.Value != null)
                {
                    await _db.UpdateBookAsync(book = rollback.Value, cancellationToken);

                    foreach (var content in book.Contents)
                        await _softDeleter.RestoreAsync(EnumerateBookFiles(book, content), cancellationToken);
                }

                await _snapshot.RevertedAsync(SnapshotType.User, book, rollback, cancellationToken);

                return book;
            }
        }

        public async Task<Book> UpdateAsync(int id, BookBase model, CancellationToken cancellationToken = default)
        {
            using (await _locker.EnterAsync(id, cancellationToken))
            {
                var book = await GetAsync(id, cancellationToken);

                _mapper.Map(model, book);

                await _db.UpdateBookAsync(book, cancellationToken);
                await _snapshot.ModifiedAsync(SnapshotType.User, book, cancellationToken);

                return book;
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            using (await _locker.EnterAsync(id, cancellationToken))
            {
                var book = await GetAsync(id, cancellationToken);

                await _vote.DeleteAsync(book, cancellationToken);
                await _db.DeleteBookAsync(book, cancellationToken);
                await _snapshot.DeletedAsync(SnapshotType.User, book, cancellationToken);

                foreach (var content in book.Contents)
                    await _softDeleter.DeleteAsync(EnumerateBookFiles(book, content), cancellationToken);
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        static IEnumerable<string> EnumerateBookFiles(Book book, BookContent content)
        {
            for (var i = 0; i < content.PageCount; i++)
                yield return $"{book.Id}/{content.Id}/{i}";
        }

        public async Task<Vote> VoteAsync(int id, VoteType? type, CancellationToken cancellationToken = default)
        {
            using (await _locker.EnterAsync(id, cancellationToken))
            {
                var book = await GetAsync(id, cancellationToken);
                var vote = await _vote.SetAsync(book, type, cancellationToken);

                if (vote == null)
                    return null;

                // score is updated
                await _db.UpdateBookAsync(book, cancellationToken);

                return vote;
            }
        }

        public async Task<(Book, BookContent)> CreateAsync(BookBase bookModel, BookContentBase contentModel, UploadTask uploadTask, CancellationToken cancellationToken = default)
        {
            var book    = _mapper.Map<Book>(bookModel);
            var content = _mapper.Map<BookContent>(contentModel);

            content.Id        = (int) Extensions.Timestamp; // assign a practically random ID because this is not autogenerated (nested)
            content.PageCount = uploadTask.FileCount;

            book.Contents = new[] { content };

            await _db.UpdateBookAsync(book, cancellationToken);
            await _snapshot.CreatedAsync(SnapshotType.User, book, cancellationToken);

            return (book, content);
        }

        public async Task<(Book, BookContent)> AddContentAsync(int id, BookContentBase model, UploadTask uploadTask, CancellationToken cancellationToken = default)
        {
            using (await _locker.EnterAsync(id, cancellationToken))
            {
                var book = await GetAsync(id, cancellationToken);

                var content = _mapper.Map<BookContent>(model);

                content.Id        = (int) Extensions.Timestamp; // assign a practically random ID because this is not autogenerated (nested)
                content.PageCount = uploadTask.FileCount;

                book.Contents = book.Contents.Append(content).ToArray();

                await _db.UpdateBookAsync(book, cancellationToken);
                await _snapshot.ModifiedAsync(SnapshotType.User, book, cancellationToken);

                return (book, content);
            }
        }

        public async Task<BookContent> UpdateContentAsync(int id, int contentId, BookContentBase model, CancellationToken cancellationToken = default)
        {
            using (await _locker.EnterAsync(id, cancellationToken))
            {
                var (book, content) = await GetContentAsync(id, contentId, cancellationToken);

                _mapper.Map(model, content);

                await _db.UpdateBookAsync(book, cancellationToken);
                await _snapshot.ModifiedAsync(SnapshotType.User, book, cancellationToken);

                return content;
            }
        }

        public async Task RemoveContentAsync(int id, int contentId, CancellationToken cancellationToken = default)
        {
            using (await _locker.EnterAsync(id, cancellationToken))
            {
                var (book, content) = await GetContentAsync(id, contentId, cancellationToken);

                if (book.Contents.Length == 1)
                {
                    // delete the entire book
                    await _db.DeleteBookAsync(book, cancellationToken);
                    await _snapshot.DeletedAsync(SnapshotType.User, book, cancellationToken);
                }
                else
                {
                    // remove the content
                    book.Contents = book.Contents.Where(c => c != content).ToArray();

                    await _db.UpdateBookAsync(book, cancellationToken);
                    await _snapshot.ModifiedAsync(SnapshotType.User, book, cancellationToken);
                }

                await _softDeleter.DeleteAsync(EnumerateBookFiles(book, content), cancellationToken);
            }
        }
    }
}