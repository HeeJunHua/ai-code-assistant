using System.Threading.Tasks;
using AICodeAssistant.Api.Data;
using AICodeAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AICodeAssistant.Api.Services;

public class QueryLogService : IQueryLogService
{
    private readonly AppDbContext _context;

    public QueryLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogQueryAsync(QueryLog log)
    {
        _context.QueryLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
