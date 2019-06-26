using Orleans;
using SharedKernel;
using System;
using System.Threading.Tasks;
using WebBookmarks.Domain;

namespace WebBookmarks.GrainInterfaces
{
    public interface IBookmarkStorageGrain : IGrainWithGuidKey
    {
        Task UpdateBookmarkStorage(BookmarkStorageState bookmarkStorage);
        Task<BookmarkStorageState> GetBookmarkStorage();
        Task<OperationResult> InsertBookmark(Bookmark newBookmark, Guid? bookmarkGroupId = null);
        Task<OperationResult> InsertBookmarkGroup(BookmarkGroup bookmarkGroup);
    }
}
