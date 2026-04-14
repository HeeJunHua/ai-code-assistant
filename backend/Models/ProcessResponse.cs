namespace AICodeAssistant.Api.Models;

public class ProcessResponse
{
    public string Result { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
}