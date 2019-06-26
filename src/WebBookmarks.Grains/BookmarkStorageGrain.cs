using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using SharedKernel;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebBookmarks.Domain;
using WebBookmarks.GrainInterfaces;

namespace WebBookmarks.Grains
{
    [StorageProvider(ProviderName = "mongodb")]
    public class BookmarkStorage : Grain<BookmarkStorageState>, IBookmarkStorageGrain
    {
        private readonly ILogger<BookmarkStorage> _logger;

        public async override Task OnActivateAsync()
        {
            await ReadStateAsync();
            await base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            return base.OnDeactivateAsync();
        }

        public BookmarkStorage(ILogger<BookmarkStorage> logger)
        {
            _logger = logger;
        }

        public Task UpdateBookmarkStorage(BookmarkStorageState bookmarkStorage)
        {
            State = bookmarkStorage;

            return WriteStateAsync();
        }

        public Task<BookmarkStorageState> GetBookmarkStorage()
        {
            return Task.FromResult(State);
        }

        public async Task<OperationResult> InsertBookmark(Bookmark newBookmark, Guid? bookmarkGroupId = null)
        {
            if (newBookmark is null)
            {
                throw new ArgumentNullException(nameof(newBookmark));
            }

            OperationResult operationResult = true;

            if (bookmarkGroupId is null || bookmarkGroupId == Guid.Empty)
            {
                State.Bookmarks.Add(newBookmark);

                await WriteStateAsync();
                return operationResult;
            }
            else
            {
                var bookmarkGroup = State.BookmarkGroups
                                         .FirstOrDefault(s => s.BookmarkGroupId == bookmarkGroupId);

                if (bookmarkGroup is null)
                {
                    operationResult.Success = false;
                    operationResult.AddMessage($"The {nameof(bookmarkGroupId)} with Id => {bookmarkGroupId}" +
                        $" is not exist.", HttpStatusCode.NotFound);

                    return operationResult;
                }
                else
                {
                    bookmarkGroup.Bookmarks.Add(newBookmark);

                    await WriteStateAsync();
                    return operationResult;
                }
            }
        }

        public async Task<OperationResult> InsertBookmarkGroup(BookmarkGroup bookmarkGroup)
        {
            if (bookmarkGroup is null)
            {
                throw new ArgumentNullException(nameof(bookmarkGroup));
            }

            OperationResult operationResult = true;

            State.BookmarkGroups.Add(bookmarkGroup);

            await WriteStateAsync();
            return operationResult;
        }
    }
}
