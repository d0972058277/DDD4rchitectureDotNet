using Microsoft.EntityFrameworkCore;
using Project.Domain.SomethingContext.Models;
using Project.Infrastructure;

namespace Project.Integration.Tests;

public static class DbContextExtensions
{
    public static async Task ResetDatabaseAsync(this ProjectDbContext dbContext)
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    public static async Task InitDataAsync(this ProjectDbContext dbContext, SomethingAggregate aggregate)
    {
        dbContext.Add(aggregate);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }
}
