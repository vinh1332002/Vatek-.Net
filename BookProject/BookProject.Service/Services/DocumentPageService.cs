using Aspose.Pdf;
using BookProject.Common.Enums;
using BookProject.Data;
using BookProject.Data.Models;
using BookProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text;

namespace BookProject.Service.Services
{
    public class DocumentPageService : IDocumentPageService
    {
        protected readonly BookProjectContext _context;

        public DocumentPageService(BookProjectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocumentPage>> GetAllDocumentPages()
        {
            return await _context.DocumentPages.ToListAsync();
        }

        public async Task DeleteDocumentPage(int documentPageId)
        {
            var documentPage = await _context.DocumentPages.FindAsync(documentPageId);

            if (documentPage != null)
            {
                _context.DocumentPages.Remove(documentPage);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Your document page is not found !");
            }
        }

        public async Task<DocumentPage> GetDocumentPage(int documentId, int pageNumber)
        {
            var existingDocument = await _context.Documents.Include(d => d.DocumentPages)
                .FirstOrDefaultAsync(e => e.Id == documentId);

            if (existingDocument == null)
            {
                throw new Exception("Your document page is not found !");
            }
            var existingDocumentPage = existingDocument.DocumentPages.FirstOrDefault(p => p.Page == pageNumber);

            if (existingDocumentPage == null)
            {
                throw new Exception("Document page with number {pageNumber} not found");
            }

            return existingDocumentPage;
        }

        public async Task<DocumentPage> UpdateDocumentPage(IFormFile inputFile, int documentId, int pageNumber)
        {
            if (inputFile.Length == 0)
            {
                throw new Exception("No file uploaded!");
            }

            var existingDocument = await _context.Documents.Include(d => d.DocumentPages)
                   .FirstOrDefaultAsync(p => p.Id == documentId);

            if (existingDocument == null)
            {
                throw new Exception("Your document page is not found !");
            }

            var existingDocumentPage = existingDocument.DocumentPages.FirstOrDefault(p => p.Page == pageNumber);

            if (existingDocumentPage == null)
            {
                throw new Exception("Document page with number {pageNumber} not found");
            }

            FileTypeEnum fileType = existingDocument.FileType;

            string fileExtension = Path.GetExtension(inputFile.FileName).ToLowerInvariant();

            if (!IsValidFileFormat(fileExtension))
            {
                throw new Exception("Unsupported file format. Only PDF, txt and Excel files are allowed.");
            }

            if (GetFileType(fileExtension) != fileType)
            {
                throw new Exception("The document page only support the appropriate document file type only!");
            }

            using (var stream = inputFile.OpenReadStream())
            {
                if (fileExtension == ".pdf")
                {
                    using (var pdfDocument = new Document(stream))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            Document pdfPage = new Document();
                            pdfPage.Pages.Add(pdfDocument.Pages[1]);
                            pdfPage.Save(memoryStream);

                            existingDocumentPage.FilePath = memoryStream.ToArray();
                            existingDocumentPage.UpdatedAt = DateTime.Now;

                            _context.DocumentPages.Update(existingDocumentPage);

                            await _context.SaveChangesAsync();

                            return existingDocumentPage;
                        }
                    }
                }
                else if (fileExtension == ".xlsx")
                {
                    using (var excelPackage = new ExcelPackage(stream))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            ExcelPackage excelPage = new ExcelPackage();
                            excelPage.Workbook.Worksheets.Add("Sheet1", excelPackage.Workbook.Worksheets[0]);

                            excelPage.SaveAs(memoryStream);

                            existingDocumentPage.FilePath = memoryStream.ToArray();
                            existingDocumentPage.UpdatedAt = DateTime.Now;

                            _context.DocumentPages.Update(existingDocumentPage);

                            await _context.SaveChangesAsync();

                            return existingDocumentPage;
                        }
                    }
                }
                else if (fileExtension == ".txt")
                {
                    const int linesPerPage = 10;
                    int linesRead = 0;

                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        StringBuilder fileContent = new StringBuilder();

                        while ((line = reader.ReadLine()) != null)
                        {
                            fileContent.AppendLine(line);
                            linesRead++;

                            if (linesRead >= linesPerPage)
                            {
                                existingDocumentPage.FilePath = Encoding.UTF8.GetBytes(fileContent.ToString().Trim());
                                existingDocumentPage.UpdatedAt = DateTime.Now;

                                _context.DocumentPages.Update(existingDocumentPage);

                                await _context.SaveChangesAsync();

                                return existingDocumentPage;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public async Task<DocumentPage> CreateDocumentPage(IFormFile inputFile, int documentId)
        {
            if (inputFile.Length == 0)
            {
                throw new Exception("No file uploaded!");
            }

            var existingDocument = await _context.Documents.Include(d => d.DocumentPages).FirstOrDefaultAsync(p => p.Id == documentId);

            if (existingDocument == null)
            {
                throw new Exception("Document not found");
            }

            FileTypeEnum fileType = existingDocument.FileType;

            int highestPageNumber = _context.Documents.Include(e => e.DocumentPages)
                .Where(p => p.Id == documentId)
                .SelectMany(d => d.DocumentPages)
                .Max(p => p.Page);

            int createPageNumber = highestPageNumber + 1;

            string fileExtension = Path.GetExtension(inputFile.FileName).ToLowerInvariant();

            if (!IsValidFileFormat(fileExtension))
            {
                throw new Exception("Unsupported file format. Only PDF, txt and Excel files are allowed.");
            }

            if (GetFileType(fileExtension) != fileType)
            {
                throw new Exception("The document page only support {fileType} only!");
            }

            using (var stream = inputFile.OpenReadStream())
            {
                if (fileExtension == ".pdf")
                {
                    using (var pdfDocument = new Document(stream))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            Document pdfPage = new Document();
                            pdfPage.Pages.Add(pdfDocument.Pages[1]);
                            pdfPage.Save(memoryStream);

                            var newDocumentPage = new DocumentPage
                            {
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                FilePath = memoryStream.ToArray(),
                                Page = createPageNumber,
                                Bookmark = new BookmarkPage
                                {
                                    BookmarkCheck = BookmarkCheckEnum.NotChecked
                                }
                            };

                            newDocumentPage.Bookmark.DocumentPageId = newDocumentPage.Id;

                            existingDocument.DocumentPages.Add(newDocumentPage);

                            await _context.SaveChangesAsync();

                            return newDocumentPage;
                        }
                    }
                }
                else if (fileExtension == ".xlsx")
                {
                    using (var excelPackage = new ExcelPackage(stream))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            ExcelPackage excelPage = new ExcelPackage();
                            excelPage.Workbook.Worksheets.Add("Sheet1", excelPackage.Workbook.Worksheets[0]);

                            excelPage.SaveAs(memoryStream);

                            var newDocumentPage = new DocumentPage
                            {
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                FilePath = memoryStream.ToArray(),
                                Page = createPageNumber,
                                Bookmark = new BookmarkPage
                                {
                                    BookmarkCheck = BookmarkCheckEnum.NotChecked
                                }
                            };

                            newDocumentPage.Bookmark.DocumentPageId = newDocumentPage.Id;

                            existingDocument.DocumentPages.Add(newDocumentPage);

                            await _context.SaveChangesAsync();

                            return newDocumentPage;
                        }
                    }
                }
                else if (fileExtension == ".txt")
                {
                    const int linesPerPage = 10;
                    int linesRead = 0;

                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        StringBuilder fileContent = new StringBuilder();

                        while ((line = reader.ReadLine()) != null)
                        {
                            fileContent.AppendLine(line);
                            linesRead++;

                            if (linesRead >= linesPerPage)
                            {
                                var newDocumentPage = new DocumentPage
                                {
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                    FilePath = Encoding.UTF8.GetBytes(fileContent.ToString().Trim()),
                                    Page = createPageNumber,
                                    Bookmark = new BookmarkPage
                                    {
                                        BookmarkCheck = BookmarkCheckEnum.NotChecked
                                    }
                                };

                                newDocumentPage.Bookmark.DocumentPageId = newDocumentPage.Id;

                                existingDocument.DocumentPages.Add(newDocumentPage);

                                await _context.SaveChangesAsync();

                                return newDocumentPage;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private FileTypeEnum GetFileType(string extension)
        {
            return extension == ".pdf" ? FileTypeEnum.pdf :
                extension == ".xlsx" ? FileTypeEnum.xlsx : FileTypeEnum.txt;
        }

        private bool IsValidFileFormat(string extension)
        {
            return extension == ".pdf" || extension == ".xlsx" || extension == ".txt";
        }
    }
}
