
using XmlUploader.Web.Constants;
using XmlUploader.Web.Services;
using XmlUploader.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IFileApiService, FileApiService>();

// Registrer IHttpClintFactory to use it in BaseApiRequest
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// set API URLs from appsettings
Constants.FileApiUrl = builder.Configuration["ApiServiceUrls:FileApiUrl"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Files}/{action=Upload}/{id?}");

app.Run();
