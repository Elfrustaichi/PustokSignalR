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
}).AddDefaultTokenProviders().AddEntityFrameworkStores<DataContext>();


builder.Services.AddScoped<LayoutService>();

builder.Services.AddHttpContextAccessor();


var app = builder.Build();



app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();

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