var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Configuration
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
.AddEnvironmentVariables();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "index-route",
    pattern: "Index",
    defaults: new { controller = "Index", action = "Index" });

app.MapControllerRoute(
    name: "conversation-route",
    pattern: "Conversation",
    defaults: new { controller = "Conversation", action = "Conversation" });

app.MapControllerRoute(
    name: "review-route",
    pattern: "Review",
    defaults: new { controller = "Review", action = "Review" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Index}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "unauthorized-route",
    pattern: "Unauthorized",
    defaults: new { controller = "Index", action = "Unauthorized" });

app.MapControllerRoute(
    name: "auth-route",
    pattern: "{authToken?}",
    defaults: new { controller = "Authentication", action = "Authenticate" });

app.MapControllerRoute(
    name: "bot-route",
    pattern: "{botId?}/{authToken?}",
    defaults: new { controller = "Authentication", action = "AuthenticateBot" });

app.Run();
