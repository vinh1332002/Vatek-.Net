using BookProject.Data.Models;
using BookProject.Service.DTOs.Document;
using Microsoft.AspNetCore.Http;

namespace BookProject.Service.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentInformation> CreateDocument(CreateDocumentRequest document, IFormFile inputFile);
        Task<IEnumerable<DocumentInformation>> GetAllDocuments();
        Task<DocumentQueryResponse> GetPaginationAsync(DocumentQueryRequest queryModel);
        Task DeleteDocument(int documentId);
        Task<DocumentInformation> UpdateDocument(UpdateDocumentRequest document, int Id);
    }
}
