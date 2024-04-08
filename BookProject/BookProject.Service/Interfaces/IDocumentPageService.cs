using BookProject.Data.Models;
using Microsoft.AspNetCore.Http;

namespace BookProject.Service.Interfaces
{
    public interface IDocumentPageService
    {
        Task<IEnumerable<DocumentPage>> GetAllDocumentPages();
        Task DeleteDocumentPage(int documentPageId);
        Task<DocumentPage> UpdateDocumentPage(IFormFile inputFile, int documentId, int pageNumber);
        Task<DocumentPage> CreateDocumentPage(IFormFile inputFile, int documentId);
        Task<DocumentPage> GetDocumentPage(int documentId, int pageNumber);
    }
}
