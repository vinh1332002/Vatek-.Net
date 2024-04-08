using BookProject.Common.Enums;
using BookProject.Data;
using BookProject.Data.Models;
using BookProject.Service.DTOs.Bookmark;
using BookProject.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookProject.Service.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly BookProjectContext _context;

        public BookmarkService(BookProjectContext context)
        {
            _context = context;
        }

        public async Task<BookmarkPage> SaveBookmarkPage(string saveBookmark, int documentId, int pageNumber)
        {
            var existingDocument = await _context.Documents.Include(e => e.DocumentPages).FirstOrDefaultAsync(p => p.Id == documentId);

            if (existingDocument == null)
            {
                throw new Exception("Document is not found");
            }

            var existingDocumentPage = existingDocument.DocumentPages.AsQueryable().Include(p => p.Bookmark.BookmarkCheck).FirstOrDefault(p => p.Page == pageNumber);

            if (existingDocumentPage == null)
            {
                throw new Exception("Document page with number {pageNumber} not found");
            }

            BookmarkCheckEnum bookmarkCheck;

            if (saveBookmark.Equals("Checked", StringComparison.OrdinalIgnoreCase))
            {
                bookmarkCheck = BookmarkCheckEnum.Checked;
            }
            else if (saveBookmark.Equals("NotChecked", StringComparison.OrdinalIgnoreCase))
            {
                bookmarkCheck = BookmarkCheckEnum.NotChecked;
            }
            else
            {
                throw new Exception("Invalid updateRequest value");
            }

            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.DocumentPageId == existingDocumentPage.Id);

            if (bookmark == null)
            {
                throw new Exception("Bookmark not found for this document page");
            }

            bookmark.BookmarkCheck = bookmarkCheck;

            _context.Bookmarks.Update(bookmark);

            await _context.SaveChangesAsync();

            return bookmark;
        }

        public async Task<IEnumerable<BookmarkPage>> GetAllBookmarks()
        {
            return await _context.Bookmarks.Where(e => e.BookmarkCheck == BookmarkCheckEnum.Checked).ToListAsync();
        }

        public async Task<DocumentPage> GetBookmarkPage(int documentId, int pageNumber)
        {
            var existingDocument = await _context.Documents.Include(e => e.DocumentPages)
                .Where(d => d.DocumentPages.Any(dp => dp.Bookmark.BookmarkCheck == BookmarkCheckEnum.Checked))
                .FirstOrDefaultAsync(p => p.Id == documentId);

            if (existingDocument == null)
            {
                throw new Exception("Document is not found");
            }

            var existingDocumentPage = existingDocument.DocumentPages.AsQueryable().Include(p => p.Bookmark)
                .FirstOrDefault(p => p.Page == pageNumber);

            if (existingDocumentPage == null)
            {
                throw new Exception("Document page with number {pageNumber} not found or did not have any bookmark check");
            }

            var existingBookmark = _context.Bookmarks.Where(d => d.DocumentPageId == existingDocumentPage.Id);

            if (existingBookmark.Any(d => d.BookmarkCheck != BookmarkCheckEnum.Checked))
            {
                throw new Exception("Document page did not have any bookmark check");
            }

            var bookmark = await _context.DocumentPages
                .FirstOrDefaultAsync(b => b.Id == existingDocumentPage.Id);

            if (bookmark == null)
            {
                throw new Exception("Bookmark not found for this document page");
            }

            return bookmark;
        }

        public async Task<BookmarkQueryResponse> GetPaginationBookmarkAsync(BookmarkQueryRequest queryModel)
        {
            var documents = await _context.Documents.Include(m => m.Category)
                .Include(d => d.DocumentPages)
                .ThenInclude(dp => dp.Bookmark)
                .Where(d => d.DocumentPages.Any(dp => dp.Bookmark.BookmarkCheck == BookmarkCheckEnum.Checked))
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(queryModel.Title))
            {
                var nameToQuery = queryModel.Title.Trim().ToLower();
                documents = documents?.Where(x => x.Title.ToLower().Contains(nameToQuery))?.ToList();
            }

            if (!string.IsNullOrWhiteSpace(queryModel.Author))
            {
                var nameToQuery = queryModel.Author.Trim().ToLower();
                documents = documents?.Where(x => x.Author.ToLower().Contains(nameToQuery))?.ToList();
            }

            if (queryModel.CategoryID.HasValue)
            {
                var categoryID = queryModel.CategoryID.Value;
                documents = documents?.Where(x => x.Category.Id == categoryID)?.ToList();
            }

            queryModel.SortOption ??= DocumentSortEnum.NameAcsending;

            switch (queryModel.SortOption.Value)
            {
                case DocumentSortEnum.NameAcsending:
                    documents = documents?.OrderBy(x => x.Title)?.ToList();
                    break;
                case DocumentSortEnum.NameDesending:
                    documents = documents?.OrderByDescending(x => x.Title)?.ToList();
                    break;
                case DocumentSortEnum.CategoryNameDesending:
                    documents = documents?.OrderByDescending(x => x.Category.CategoryName)?.ToList();
                    break;
                case DocumentSortEnum.CategoryNameAcsending:
                    documents = documents?.OrderBy(x => x.Category.CategoryName)?.ToList();
                    break;
                default: break;
            }

            if (documents == null || documents.Count == 0)
            {
                return new BookmarkQueryResponse
                {
                    Documents = new List<BookmarkQueryListResponse>(),
                    TotalPage = 1,
                    TotalDocumentCount = 0,
                    QueryModel = queryModel
                };
            }

            var output = new BookmarkQueryResponse();

            output.TotalDocumentCount = documents.Count;
            output.TotalPage = (output.TotalDocumentCount - 1) / queryModel.PageSize + 1;

            if (queryModel.PageNumber > output.TotalPage)
                queryModel.PageNumber = output.TotalPage;

            output.Documents = documents
                .Select(d => new BookmarkQueryListResponse
                {
                    Id = d.Id,
                    CategoryName = d.Category?.CategoryName,
                    Title = d.Title,
                    Author = d.Author,
                    DocumentPages = d.DocumentPages
                        .Where(dp => dp.Bookmark?.BookmarkCheck == BookmarkCheckEnum.Checked)
                        .Select(dp => dp.Page) // Include only PageNumber
                        .ToList(),
                })
                .
                Skip((queryModel.PageNumber - 1) * queryModel.PageSize).Take(queryModel.PageSize).ToList();

            output.QueryModel = queryModel;

            return output;
        }
    }
}
