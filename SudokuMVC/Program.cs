using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using YourProjectNamespace.Models;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Configure EF Core with SQLite.
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
    context.Database.Migrate();
    
    // For testing: remove existing entries to force seeding.
    var existing = context.LeaderboardEntries.ToList();
    if (existing.Any())
    {
        context.LeaderboardEntries.RemoveRange(existing);
        context.SaveChanges();
    }
    
    // Seed the database with 10 entries per difficulty.
    var entries = new List<LeaderboardEntry>();

    // Each seeded entry will have a StopwatchValue of 1 second.
    // Seed 10 entries for Easy difficulty.
    for (int i = 1; i <= 10; i++)
    {
        entries.Add(new LeaderboardEntry
        {
            PlayerName = "Test",
            Difficulty = "Easy",
            StopwatchValue = 2,
            DateAchieved = DateTime.Now.AddDays(-i)
        });
    }

    // Seed 10 entries for Medium difficulty.
    for (int i = 1; i <= 10; i++)
    {
        entries.Add(new LeaderboardEntry
        {
            PlayerName = "Test",
            Difficulty = "Medium",
            StopwatchValue = 2,
            DateAchieved = DateTime.Now.AddDays(-i)
        });
    }

    // Seed 10 entries for Hard difficulty.
    for (int i = 1; i <= 10; i++)
    {
        entries.Add(new LeaderboardEntry
        {
            PlayerName = "Test",
            Difficulty = "Hard",
            StopwatchValue = 3,
            DateAchieved = DateTime.Now.AddDays(-i)
        });
    }

    context.LeaderboardEntries.AddRange(entries);
    context.SaveChanges();
}

app.Run();
