using BookProject.Common.Enums;
using BookProject.Data.Models;
using BookProject.Service.DTOs.Bookmark;
using BookProject.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookProject.Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;

        public BookmarkController(IBookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        [HttpPut]
        [Route("bookmark/save/{documentId}/{pageNumber}")]
        public async Task<IActionResult> SaveBookmarkPage([FromForm] string updateRequest, int documentId, int pageNumber)
        {
            if (updateRequest == null) return BadRequest();

            try
            {
                var updatedDocument = await _bookmarkService.SaveBookmarkPage(updateRequest, documentId, pageNumber);

                if (updatedDocument == null)
                {
                    return NotFound("Can not change status bookmark of page");
                }

                return Ok(new
                {
                    Id = updatedDocument.Id,
                    DocumentPageId = updatedDocument.DocumentPageId,
                    BookmarkCheck = updatedDocument.BookmarkCheck,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while change the bookmark status: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("bookmark/get/{documentId}/{pageNumber}")]
        public async Task<IActionResult> GetBookmarkPage(int documentId, int pageNumber)
        {
            try
            {
                var getDocument = await _bookmarkService.GetBookmarkPage(documentId, pageNumber);

                if (getDocument == null)
                {
                    return NotFound("Can not change status bookmark of page");
                }

                return Ok(new
                {
                    Id = getDocument.Id,
                    Page = getDocument.Page,
                    FilePath = getDocument.FilePath,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while change the bookmark status: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("bookmark/getall")]
        public async Task<ActionResult<IEnumerable<BookmarkPage>>> GetAllBookmarks()
        {
            try
            {
                var bookmarks = await _bookmarkService.GetAllBookmarks();

                if (bookmarks == null)
                {
                    return NotFound();
                }
                return Ok(bookmarks.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving documents: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("bookmark/getQuery")]
        public async Task<ActionResult> GetBookmarkQueryAsync(int pageNumber, int pageSize,
            string? title, string? author, int? categoryId, DocumentSortEnum? sortOption)
        {
            var queryModel = new BookmarkQueryRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Author = author,
                Title = title,
                CategoryID = categoryId,
                SortOption = sortOption
            };

            var result = await _bookmarkService.GetPaginationBookmarkAsync(queryModel);
            return Ok(result);
        }
    }
}
