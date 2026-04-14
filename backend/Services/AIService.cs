using System.Net.Http;
using System.Text;
using System.Text.Json;
using AICodeAssistant.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AICodeAssistant.Api.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly AIOptions _options;
    private readonly ILogger<AIService> _logger;

    public AIService(
        HttpClient httpClient,
        IOptions<AIOptions> options,
        ILogger<AIService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> ProcessCodeAsync(string code, string action, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code cannot be empty", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException("Action cannot be empty", nameof(action));
        }

        try
        {
            // Build the prompt
            string prompt = $"You are a senior developer. {action} this code:\n{code}";

            // Create the Ollama request
            var request = new OllamaRequest
            {
                Model = _options.Model,
                Prompt = prompt,
                Stream = false,
                Options = new OllamaOptions
                {
                    NumPredict = _options.MaxTokens,
                    Temperature = _options.Temperature
                }
            };

            // Serialize the request
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string requestJson = JsonSerializer.Serialize(request, jsonOptions);

            _logger.LogInformation("Sending request to Ollama API: {Endpoint}", _options.Endpoint);

            // Send the request
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_options.Endpoint, content, cancellationToken);

            // Read the response
            string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Ollama API request failed with status {StatusCode}: {Response}", 
                    response.StatusCode, responseJson);
                throw new Exception($"Ollama API request failed: {response.StatusCode}. Response: {responseJson}");
            }

            // Deserialize the response
            var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseJson, jsonOptions);

            if (ollamaResponse == null)
            {
                throw new Exception("Ollama API returned null response");
            }

            if (!ollamaResponse.Done)
            {
                throw new Exception("Ollama API response not complete");
            }

            string result = ollamaResponse.Response;
            _logger.LogInformation("Ollama processing completed successfully for action: {Action}", action);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while calling Ollama API");
            throw new Exception("Failed to connect to Ollama service. Please check if Ollama is running on localhost:11434.", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error from Ollama API response");
            throw new Exception("Failed to parse Ollama response. Please try again.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while processing code with Ollama");
            throw;
        }
    }
}

public class AIOptions
{
    public string Endpoint { get; set; } = "http://localhost:11434/api/generate";
    public string Model { get; set; } = "llama3";
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
}