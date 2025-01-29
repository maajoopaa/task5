var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IBookService,BookService>();
builder.Services.AddSingleton<ISettingsService,SettingsService>();
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
    pattern: "{controller=Home}/{action=MainPage}/{id?}");

app.Run();
public interface IBookService
{
    public int Id { get; set; }
}
public class BookService : IBookService
{
    public int Id { get; set; } = -1;
}
public interface ISettingsService
{
    public string Language { get; set; }
    public int Seed { get; set; }
    public string Likes { get; set; }
    public string Reviews { get; set; }
}
public class SettingsService : ISettingsService
{
    public string Language { get; set; } = "ru";
    public int Seed { get; set; } = 0;
    public string Likes { get; set; } = "1";
    public string Reviews { get; set; } = "1";
}
