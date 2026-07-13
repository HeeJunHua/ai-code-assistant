using Microsoft.EntityFrameworkCore;
using AICodeAssistant.Api.Models;

namespace AICodeAssistant.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<QueryLog> QueryLogs { get; set; } = null!;
}
