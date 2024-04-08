namespace BookProject.Service.DTOs.Bookmark
{
    public class BookmarkQueryListResponse
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public List<int> DocumentPages { get; set; }
    }
}
