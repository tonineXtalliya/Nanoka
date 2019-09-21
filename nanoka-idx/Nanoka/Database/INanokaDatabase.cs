using System;
using System.Threading;
using System.Threading.Tasks;
using Nanoka.Models;

namespace Nanoka.Database
{
    public interface INanokaDatabase : IDisposable
    {
        Task MigrateAsync(CancellationToken cancellationToken = default);
        Task ResetAsync(CancellationToken cancellationToken = default);

        Task<User> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<User> GetUserByNameAsync(string username, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(User user, CancellationToken cancellationToken = default);

        Task<Book> GetBookAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateBookAsync(Book book, CancellationToken cancellationToken = default);
        Task DeleteBookAsync(Book book, CancellationToken cancellationToken = default);

        Task<Image> GetImageAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateImageAsync(Image image, CancellationToken cancellationToken = default);
        Task DeleteImageAsync(Image image, CancellationToken cancellationToken = default);

        Task<Snapshot<T>> GetSnapshotAsync<T>(string id, string entityId, CancellationToken cancellationToken = default);
        Task<Snapshot<T>[]> GetSnapshotsAsync<T>(string entityId, int start, int count, bool chronological, CancellationToken cancellationToken = default);
        Task UpdateSnapshotAsync<T>(Snapshot<T> snapshot, CancellationToken cancellationToken = default);

        Task<Vote> GetVoteAsync(string userId, NanokaEntity entity, string entityId, CancellationToken cancellationToken = default);
        Task UpdateVoteAsync(Vote vote, CancellationToken cancellationToken = default);
        Task DeleteVoteAsync(Vote vote, CancellationToken cancellationToken = default);
        Task<int> DeleteVotesAsync(NanokaEntity entity, string entityId, CancellationToken cancellationToken = default);

        Task AddDeleteFilesAsync(string[] filenames, DateTime softDeleteTime, CancellationToken cancellationToken = default);
        Task RemoveDeleteFileAsync(string[] filenames, CancellationToken cancellationToken = default);
        Task<string[]> GetAndRemoveDeleteFilesAsync(DateTime maxSoftDeleteTime, CancellationToken cancellationToken = default);
    }
}