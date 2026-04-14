namespace AICodeAssistant.Api.Services;

public interface IAIService
{
    Task<string> ProcessCodeAsync(string code, string action, CancellationToken cancellationToken = default);
}