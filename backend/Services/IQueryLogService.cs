using System.Threading.Tasks;
using AICodeAssistant.Api.Models;

namespace AICodeAssistant.Api.Services;

public interface IQueryLogService
{
    Task LogQueryAsync(QueryLog log);
}
