using BookProject.Data.Models;

namespace BookProject.Service.DTOs.Document
{
    public class CreateDocumentRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<DocumentPage> Pages { get; set; } = new List<DocumentPage>();
    }
}
