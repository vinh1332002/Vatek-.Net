namespace BookProject.Service.DTOs.Bookmark
{
    public class BookmarkQueryResponse
    {
        public List<BookmarkQueryListResponse> Documents { get; set; }
        public int TotalDocumentCount { get; set; }
        public int TotalPage { get; set; }
        public BookmarkQueryRequest QueryModel { get; set; }
    }
}
