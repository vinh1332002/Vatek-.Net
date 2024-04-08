using BookProject.Common.Enums;
using BookProject.Data.Models;
using BookProject.Service.DTOs.Document;
using BookProject.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost]
    [Route("documents")]
    public async Task<IActionResult> CreateDocument([FromForm] CreateDocumentRequest request, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select a file to upload!");
            }

            var documentInformation = await _documentService.CreateDocument(request, file);

            return Ok(new
            {
                Id = documentInformation.Id,
                Title = documentInformation.Title,
                Author = documentInformation.Author,
                Category = documentInformation.Category,
                CreatedDate = documentInformation.CreatedDate,
                UpdatedDate = documentInformation.UpdatedDate,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("documents/getall")]
    public async Task<ActionResult<IEnumerable<DocumentInformation>>> GetDocuments()
    {
        try
        {
            var documents = await _documentService.GetAllDocuments();
            if (documents == null)
            {
                return NotFound();
            }
            return Ok(documents.ToList());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving documents: {ex.Message}");
        }
    }

    [HttpGet]
    [Route("documents/getQuery")]
    public async Task<ActionResult> GetDocumentQueryAsync(int pageNumber, int pageSize,
            string? title, string? author, int? categoryId, DocumentSortEnum? sortOption)
    {
        var queryModel = new DocumentQueryRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Author = author,
            Title = title,
            CategoryID = categoryId,
            SortOption = sortOption
        };

        var result = await _documentService.GetPaginationAsync(queryModel);
        return Ok(result);
    }

    [HttpDelete]
    [Route("documents/delete/{id}")]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        try
        {
            await _documentService.DeleteDocument(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting document with ID {id}: {ex.Message}");
        }
    }

    [HttpPut]
    [Route("documents/update/{id}")]
    public async Task<IActionResult> UpdateDocument([FromBody] UpdateDocumentRequest updateRequest, int id)
    {
        if (updateRequest == null) return BadRequest();

        try
        {
            var updatedDocument = await _documentService.UpdateDocument(updateRequest, id);

            if (updatedDocument == null)
            {
                return NotFound("Document not found");
            }

            return Ok(updatedDocument);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while update document with ID {id}: {ex.Message}");
        }
    }
}
