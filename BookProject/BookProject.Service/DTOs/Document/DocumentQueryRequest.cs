using BookProject.Common.Enums;
using BookProject.Common.Systems;

namespace BookProject.Service.DTOs.Document
{
    public class DocumentQueryRequest
    {
        private int _pageSize;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value < Constant.MIN_PAGE_SIZE)
                    value = Constant.MIN_PAGE_SIZE;
                else if (value > Constant.MAX_PAGE_SIZE)
                    value = Constant.MAX_PAGE_SIZE;

                _pageSize = value;
            }
        }
        public int PageNumber { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int? CategoryID { get; set; }
        public DocumentSortEnum? SortOption { get; set; }
    }
}
