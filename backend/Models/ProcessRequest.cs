namespace AICodeAssistant.Api.Models;

public class ProcessRequest
{
    public string Code { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}