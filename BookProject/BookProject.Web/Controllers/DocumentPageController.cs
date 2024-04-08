using BookProject.Data.Models;
using BookProject.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;

namespace BookProject.Web.Controllers
{
    [Route("api")]
    public class DocumentPageController : ControllerBase
    {
        private readonly IDocumentPageService _documentPageService;
        private readonly IHubContext<DocumentPageHub> _contextHub;

        public DocumentPageController(IDocumentPageService documentPageService, IHubContext<DocumentPageHub> contextHub)
        {
            _documentPageService = documentPageService;
            _contextHub = contextHub;
        }

        [HttpGet]
        [Route("documentPages/getall")]
        public async Task<ActionResult<IEnumerable<DocumentPage>>> GetDocumentPages()
        {
            try
            {
                var documentPages = await _documentPageService.GetAllDocumentPages();
                if (documentPages == null)
                {
                    return NotFound();
                }
                return Ok(documentPages.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving document pages: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("documentPages/delete/{id}")]
        public async Task<IActionResult> DeleteDocumentPage(int id)
        {
            try
            {
                await _documentPageService.DeleteDocumentPage(id);

                if (_contextHub == null)
                {
                    return NotFound("Unable to connect to the Hub");
                }

                await _contextHub.Clients.All.SendAsync("refreshDocumentPage");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting document with ID {id}: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("documentPages/get/{documentId}/{pageNumber}")]
        public async Task<IActionResult> GetOneDocumentPage(int documentId, int pageNumber)
        {
            try
            {
                var documentPage = await _documentPageService.GetDocumentPage(documentId, pageNumber);

                if (documentPage == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    Id = documentPage.Id,
                    Page = documentPage.Page,
                    FilePath = documentPage.FilePath,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while get document with ID {documentId}: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("documentPages/update/{documentId}/{pageId}")]
        public async Task<IActionResult> UpdateDocumentPage(IFormFile updateRequest, int documentId, int pageId)
        {
            if (updateRequest == null) return BadRequest();

            try
            {
                var updatedDocument = await _documentPageService.UpdateDocumentPage(updateRequest, documentId, pageId);

                if (updatedDocument == null)
                {
                    return NotFound("Document page not found");
                }

                if (_contextHub == null)
                {
                    return NotFound("Unable to connect to the Hub");
                }

                await _contextHub.Clients.All.SendAsync("refreshDocumentPage");

                return Ok(new
                {
                    Id = updatedDocument.Id,
                    Page = updatedDocument.Page,
                    CreatedAt = updatedDocument.CreatedAt,
                    UpdatedAt = updatedDocument.UpdatedAt,
                    FilePath = updatedDocument.FilePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while update document with ID {documentId} and page {pageId}: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("documentPages/create/{id}")]
        public async Task<IActionResult> CreateDocumentPage([FromForm] IFormFile createRequest, int id)
        {
            if (createRequest == null) return BadRequest();

            try
            {
                var createdDocument = await _documentPageService.CreateDocumentPage(createRequest, id);

                if (createdDocument == null)
                {
                    return NotFound("Document page not found");
                }

                if (_contextHub == null)
                {
                    return NotFound("Unable to connect to the Hub");
                }

                await _contextHub.Clients.All.SendAsync("refreshDocumentPage");

                return Ok(new
                {
                    Id = createdDocument.Id,
                    Page = createdDocument.Page,
                    CreatedAt = createdDocument.CreatedAt,
                    UpdatedAt = createdDocument.UpdatedAt,
                    FilePath = createdDocument.FilePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while create document page with ID {id}: {ex.Message}");
            }
        }
    }
}
