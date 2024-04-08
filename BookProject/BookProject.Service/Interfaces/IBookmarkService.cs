using BookProject.Data.Models;
using BookProject.Service.DTOs.Bookmark;

namespace BookProject.Service.Interfaces
{
    public interface IBookmarkService
    {
        Task<BookmarkPage> SaveBookmarkPage(string saveBookmark, int documentId, int pageNumber);
        Task<IEnumerable<BookmarkPage>> GetAllBookmarks();
        Task<BookmarkQueryResponse> GetPaginationBookmarkAsync(BookmarkQueryRequest queryModel);
        Task<DocumentPage> GetBookmarkPage(int documentId, int pageNumber);
    }
}
