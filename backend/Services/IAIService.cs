using AICodeAssistant.Api.Models;

namespace AICodeAssistant.Api.Services;

public interface IAIService
{
    Task<string> ProcessCodeAsync(ProcessRequest request, CancellationToken cancellationToken = default);
}