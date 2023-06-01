using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PustokBackTask.DAL;
using PustokBackTask.Models;
using PustokBackTask.Services;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlServer("Server=ELFRUSTAICHI\\SQLEXPRESS;Database=PustokTaskDB;Trusted_Connection=true");
});

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
}).AddDefaultTokenProviders().AddEntityFrameworkStores<DataContext>();


builder.Services.AddScoped<LayoutService>();
builder.Services.AddScoped<IEmailSender,EmailSender>();

builder.Services.AddAuthentication().AddGoogle(opt =>
{
    opt.ClientId = "183091077431-vanfdi83raul7du6qrbfc0pdau6pfa87.apps.googleusercontent.com";
    opt.ClientSecret = "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";
});

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.HttpContext.Request.Path.Value.StartsWith("/manage"))
        {
            var RedirectUri = new Uri(context.RedirectUri);
            context.Response.Redirect("/manage/account/login"+RedirectUri.Query);
        }
        return Task.CompletedTask;
    };
});




var app = builder.Build();



app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();