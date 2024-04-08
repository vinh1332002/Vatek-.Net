using BookProject.Data;
using BookProject.Data.Models;
using BookProject.Service.Interfaces;
using BookProject.Service.DTOs.Document;
using Aspose.Pdf;
using Microsoft.AspNetCore.Http;
using BookProject.Common.Enums;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text;

namespace BookProject.Service.Services
{
    public class DocumentService : IDocumentService
    {
        protected readonly BookProjectContext _context;

        public DocumentService(BookProjectContext context)
        {
            _context = context;
        }

        public async Task<DocumentInformation> CreateDocument(CreateDocumentRequest document, IFormFile inputFile)
        {
            if (inputFile.Length == 0)
            {
                throw new Exception("No file uploaded!");
            }

            var category = await _context.Categories.FindAsync(document.CategoryId);
            if (category == null)
            {
                throw new Exception("Your category is not found!");
            }

            string fileExtension = Path.GetExtension(inputFile.FileName).ToLowerInvariant();

            if (!IsValidFileFormat(fileExtension))
            {
                throw new Exception("Unsupported file format. Only PDF, txt and Excel files are allowed.");
            }

            using (var stream = inputFile.OpenReadStream())
            {
                var createDocumentRequest = new DocumentInformation
                {
                    Title = document.Title,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Author = document.Author,
                    CategoryId = document.CategoryId,
                    FileType = GetFileType(fileExtension),
                    DocumentPages = await ProcessDocumentPages(stream, fileExtension)
                };

                _context.Documents.Add(createDocumentRequest);
                await _context.SaveChangesAsync();

                return createDocumentRequest;
            }
        }

        private bool IsValidFileFormat(string extension)
        {
            return extension == ".pdf" || extension == ".xlsx" || extension == ".txt";
        }

        private FileTypeEnum GetFileType(string extension)
        {
            return extension == ".pdf" ? FileTypeEnum.pdf :
                extension == ".xlsx" ? FileTypeEnum.xlsx : FileTypeEnum.txt;
        }

        private async Task<List<DocumentPage>> ProcessDocumentPages(Stream stream, string fileExtension)
        {
            List<DocumentPage> documentPages = new List<DocumentPage>();

            if (fileExtension == ".pdf")
            {
                using (var pdfDocument = new Document(stream))
                {
                    for (var page = 1; page <= pdfDocument.Pages.Count; page++)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            Document pdfPage = new Document();
                            pdfPage.Pages.Add(pdfDocument.Pages[page]);
                            pdfPage.Save(memoryStream);

                            documentPages.Add(CreateDocumentPage(memoryStream.ToArray(), page));
                        }
                    }
                }
            }
            else if (fileExtension == ".xlsx")
            {
                using (var excelPackage = new ExcelPackage(stream))
                {
                    for (int sheetIndex = 1; sheetIndex <= excelPackage.Workbook.Worksheets.Count; sheetIndex++)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            ExcelPackage excelPage = new ExcelPackage();
                            excelPage.Workbook.Worksheets.Add("Sheet1", excelPackage.Workbook.Worksheets[sheetIndex - 1]);

                            excelPage.SaveAs(memoryStream);

                            documentPages.Add(CreateDocumentPage(memoryStream.ToArray(), sheetIndex));
                        }
                    }
                }
            }
            else if (fileExtension == ".txt")
            {
                const int linesPerPage = 10;
                int linesRead = 0;
                int fileNumber = 1;

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
                            documentPages.Add(CreateDocumentPage(Encoding.UTF8.GetBytes(fileContent.ToString().Trim()), fileNumber));
                            linesRead = 0;
                            fileContent.Clear();
                            fileNumber++;
                        }
                    }

                    if (fileContent.Length > 0)
                    {
                        documentPages.Add(CreateDocumentPage(Encoding.UTF8.GetBytes(fileContent.ToString().Trim()), fileNumber));
                    }
                }
            }

            await SaveDocumentPages(documentPages);
            return documentPages;
        }

        private DocumentPage CreateDocumentPage(byte[] fileContent, int pageNumber)
        {
            return new DocumentPage
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                FilePath = fileContent,
                Page = pageNumber,
                Bookmark = new BookmarkPage
                {
                    BookmarkCheck = BookmarkCheckEnum.NotChecked
                }
            };
        }

        private async Task SaveDocumentPages(List<DocumentPage> documentPages)
        {
            foreach (var page in documentPages)
            {
                page.Bookmark.DocumentPageId = page.Id;
                _context.DocumentPages.Add(page);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<DocumentInformation> UpdateDocument(UpdateDocumentRequest document, int documentId)
        {
            var existingDocument = await _context.Documents
                   .FirstOrDefaultAsync(p => p.Id == documentId);

            if (existingDocument == null)
            {
                throw new Exception("Document not found");
            }

            var cate = await _context.Categories.FindAsync(document.CategoryId);

            if (cate == null)
            {
                throw new Exception("Your category is not found !");
            }

            existingDocument.Title = document.Title;
            existingDocument.Author = document.Author;
            existingDocument.CategoryId = document.CategoryId;
            existingDocument.UpdatedDate = DateTime.Now;

            _context.Documents.Update(existingDocument);

            await _context.SaveChangesAsync();

            return existingDocument;
        }

        public async Task<IEnumerable<DocumentInformation>> GetAllDocuments()
        {
            return await _context.Documents.Include(d => d.DocumentPages).ToListAsync();
        }

        public async Task<DocumentQueryResponse> GetPaginationAsync(DocumentQueryRequest queryModel)
        {
            var documents = await _context.Documents.Include(m => m.Category).ToListAsync();

            if (!string.IsNullOrWhiteSpace(queryModel.Title))
            {
                var nameToQuery = queryModel.Title.Trim().ToLower();
                documents = documents?.Where(x => x.Title.ToLower().Contains(nameToQuery))?.ToList();
            }

            if (!string.IsNullOrWhiteSpace(queryModel.Author))
            {
                var nameToQuery = queryModel.Author.Trim().ToLower();
                documents = documents?.Where(x => x.Author.ToLower().Contains(nameToQuery))?.ToList();
            }

            if (queryModel.CategoryID.HasValue)
            {
                var categoryID = queryModel.CategoryID.Value;
                documents = documents?.Where(x => x.Category.Id == categoryID)?.ToList();
            }

            queryModel.SortOption ??= DocumentSortEnum.NameAcsending;

            switch (queryModel.SortOption.Value)
            {
                case DocumentSortEnum.NameAcsending:
                    documents = documents?.OrderBy(x => x.Title)?.ToList();
                    break;
                case DocumentSortEnum.NameDesending:
                    documents = documents?.OrderByDescending(x => x.Title)?.ToList();
                    break;
                case DocumentSortEnum.CategoryNameDesending:
                    documents = documents?.OrderByDescending(x => x.Category.CategoryName)?.ToList();
                    break;
                case DocumentSortEnum.CategoryNameAcsending:
                    documents = documents?.OrderBy(x => x.Category.CategoryName)?.ToList();
                    break;
                default: break;
            }

            if (documents == null || documents.Count == 0)
            {
                return new DocumentQueryResponse
                {
                    Documents = new List<DocumentInformation>(),
                    TotalPage = 1,
                    TotalDocumentCount = 0,
                    QueryModel = queryModel
                };
            }

            var output = new DocumentQueryResponse();

            output.TotalDocumentCount = documents.Count;
            output.TotalPage = (output.TotalDocumentCount - 1) / queryModel.PageSize + 1;

            if (queryModel.PageNumber > output.TotalPage)
                queryModel.PageNumber = output.TotalPage;

            output.Documents = documents.Skip((queryModel.PageNumber - 1) * queryModel.PageSize).Take(queryModel.PageSize).ToList();

            output.QueryModel = queryModel;

            return output;
        }

        public async Task DeleteDocument(int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document != null)
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Your document is not found !");
            }
        }
    }
}
