using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Concrate;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<Context>();

// Configure DbContext
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Gzip Sýkýþtýrmayý Etkinleþtirin
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true; // HTTPS üzerinde de sýkýþtýrma yapmak için
    options.MimeTypes = new[] { "text/plain", "text/css", "application/javascript", "text/html", "application/xml", "text/xml", "application/json", "text/json" };
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest; // En hýzlýsý için
    // options.Level = System.IO.Compression.CompressionLevel.Optimal; // En iyi sýkýþtýrma oraný için
});

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=BLogin}/{action=Index}/{id?}");

app.Run();