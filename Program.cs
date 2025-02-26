using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using YourProjectNamespace.Models;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://+:5000");
builder.Services.AddControllersWithViews();

// configure EF Core with SQLite.
builder.Services.AddDbContext<LeaderboardDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LeaderboardDbContext>();
    // Automatically apply any pending migrations
    context.Database.Migrate();
}

/*
// clear out database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LeaderboardDbContext>();
    context.Database.Migrate();
    
    var existing = context.LeaderboardEntries.ToList();
    if (existing.Any())
    {
        context.LeaderboardEntries.RemoveRange(existing);
        context.SaveChanges();
    }
    
    context.SaveChanges();
}
*/

app.Run();
