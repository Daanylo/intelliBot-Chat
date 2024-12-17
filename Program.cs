using intelliBot.Models;
using dotenv;


var builder = WebApplication.CreateBuilder(args);

dotenv.net.DotEnv.Load();

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Bot>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Index}/{action=Index}/{id?}");

app.Run();
