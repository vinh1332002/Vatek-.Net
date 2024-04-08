using BookProject.Common.Enums;

namespace BookProject.Data.Models
{
    public class BookmarkPage
    {
        public int Id { get; set; }
        public int DocumentPageId { get; set; }
        public virtual DocumentPage DocumentPage { get; set; } = null!;
        public BookmarkCheckEnum BookmarkCheck { get; set; }
    }
}
