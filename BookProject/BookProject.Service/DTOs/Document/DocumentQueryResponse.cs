using BookProject.Data.Models;

namespace BookProject.Service.DTOs.Document
{
    public class DocumentQueryResponse
    {
        public List<DocumentInformation> Documents { get; set; }
        public int TotalDocumentCount { get; set; }
        public int TotalPage { get; set; }
        public DocumentQueryRequest QueryModel { get; set; }
    }
}
