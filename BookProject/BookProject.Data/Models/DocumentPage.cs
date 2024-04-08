namespace BookProject.Data.Models
{
    public class DocumentPage
    {
        public int Id { get; set; }
        public int Page { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[] FilePath { get; set; }
        public virtual BookmarkPage? Bookmark { get; set; }
    }
}
