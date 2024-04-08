using BookProject.Data;
using BookProject.Service.Interfaces;
using BookProject.Service.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SignalRChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
var configuration = builder.Configuration;

builder.Services.AddDbContext<BookProjectContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("BookProjectDatabase")));

builder.Services.AddControllers();

builder.Services.AddTransient<IDocumentService, DocumentService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IDocumentPageService, DocumentPageService>();
builder.Services.AddTransient<IBookmarkService, BookmarkService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();
app.MapHub<DocumentPageHub>("/signal");

app.Run();

