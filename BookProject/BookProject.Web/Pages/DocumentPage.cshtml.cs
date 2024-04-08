using BookProject.Data;
using BookProject.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BookProject.Web.Pages
{
    public class DocumentPageModel : PageModel
    {
        private readonly BookProjectContext _context;

        public DocumentPageModel(BookProjectContext context)
        {
            _context = context;
        }

        public IList<DocumentPage> DocumentPage { get; set; }

        public async Task OnGetAsync()
        {
            DocumentPage = await _context.DocumentPages.ToListAsync();
        }
    }
}