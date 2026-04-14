namespace AICodeAssistant.Api.Models;

// Ollama request model
public class OllamaRequest
{
    public string Model { get; set; } = "llama3";
    public string Prompt { get; set; } = string.Empty;
    public bool Stream { get; set; } = false;
    public OllamaOptions? Options { get; set; }
}

public class OllamaOptions
{
    public int NumPredict { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
    public int TopK { get; set; } = 40;
    public double TopP { get; set; } = 0.9;
}