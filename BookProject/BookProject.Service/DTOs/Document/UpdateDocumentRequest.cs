namespace BookProject.Service.DTOs.Document
{
    public class UpdateDocumentRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int CategoryId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
