using Microsoft.EntityFrameworkCore;
using Project.Domain.Exceptions;

namespace Project.Infrastructure;

public class ReadOnlyProjectDbContext : ProjectDbContext
{
    public ReadOnlyProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

    public override int SaveChanges()
    {
        throw new InvalidOperationDomainException($"當前的 DbContext 屬於 Read-Only 的類型，不能執行 {nameof(SaveChanges)} 的操作，請確認是否正確區分 CQRS 的使用方式。");
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new InvalidOperationDomainException($"當前的 DbContext 屬於 Read-Only 的類型，不能執行 {nameof(SaveChanges)} 的操作，請確認是否正確區分 CQRS 的使用方式。");
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationDomainException($"當前的 DbContext 屬於 Read-Only 的類型，不能執行 {nameof(SaveChangesAsync)} 的操作，請確認是否正確區分 CQRS 的使用方式。");
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationDomainException($"當前的 DbContext 屬於 Read-Only 的類型，不能執行 {nameof(SaveChangesAsync)} 的操作，請確認是否正確區分 CQRS 的使用方式。");
    }
}
