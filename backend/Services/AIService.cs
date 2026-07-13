using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AICodeAssistant.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AICodeAssistant.Api.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly AIOptions _options;
    private readonly ILogger<AIService> _logger;
    private readonly IQueryLogService _queryLogService;

    public AIService(
        HttpClient httpClient,
        IOptions<AIOptions> options,
        ILogger<AIService> logger,
        IQueryLogService queryLogService)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _queryLogService = queryLogService;
    }

    private async Task LogQuery(ProcessRequest request, string result)
    {
        await _queryLogService.LogQueryAsync(new QueryLog
        {
            InputCode = request.Code,
            Action = request.Action,
            Result = result,
            Model = request.Model ?? _options.Model,
            Endpoint = request.Endpoint ?? _options.Endpoint,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task<string> ProcessCodeAsync(ProcessRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new ArgumentException("Code cannot be empty", nameof(request.Code));
        }

        if (string.IsNullOrWhiteSpace(request.Action))
        {
            throw new ArgumentException("Action cannot be empty", nameof(request.Action));
        }

        // Build the prompt
        string prompt = $"You are a senior developer. {request.Action} this code:\n{request.Code}";
        string finalResult = string.Empty;

        // If a custom endpoint is provided, use it (generic OpenAI‑style request)
        if (!string.IsNullOrWhiteSpace(request.Endpoint))
        {
            var openAiRequest = new
            {
                model = request.Model ?? _options.Model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = _options.Temperature,
                max_tokens = _options.MaxTokens
            };

            var json = JsonSerializer.Serialize(openAiRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.Endpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrWhiteSpace(request.ApiKey))
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.ApiKey);
            }

            _logger.LogInformation("Sending request to custom AI endpoint: {Endpoint}", request.Endpoint);
            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Custom AI request failed with status {StatusCode}: {Response}",
                    response.StatusCode, responseJson);
                throw new Exception($"Custom AI request failed: {response.StatusCode}. Response: {responseJson}");
            }

            try
            {
                using var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var first = choices[0];
                    if (first.TryGetProperty("message", out var message) &&
                        message.TryGetProperty("content", out var contentProp))
                    {
                        finalResult = contentProp.GetString() ?? string.Empty;
                    }
                    else if (first.TryGetProperty("text", out var textProp))
                    {
                        finalResult = textProp.GetString() ?? string.Empty;
                    }
                }
                else
                {
                    finalResult = responseJson;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse custom AI response JSON");
                throw new Exception("Failed to parse response from custom AI provider.", ex);
            }
        }
        else
        {
            // Default: Ollama flow
            var ollamaRequest = new OllamaRequest
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

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string requestJson = JsonSerializer.Serialize(ollamaRequest, jsonOptions);

            _logger.LogInformation("Sending request to Ollama API: {Endpoint}", _options.Endpoint);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var ollamaResponseMsg = await _httpClient.PostAsync(_options.Endpoint, content, cancellationToken);
            var ollamaResponseJson = await ollamaResponseMsg.Content.ReadAsStringAsync(cancellationToken);

            if (!ollamaResponseMsg.IsSuccessStatusCode)
            {
                _logger.LogError("Ollama API request failed with status {StatusCode}: {Response}",
                    ollamaResponseMsg.StatusCode, ollamaResponseJson);
                throw new Exception($"Ollama API request failed: {ollamaResponseMsg.StatusCode}. Response: {ollamaResponseJson}");
            }

            var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(ollamaResponseJson, jsonOptions);
            if (ollamaResponse == null)
            {
                throw new Exception("Ollama API returned null response");
            }

            if (!ollamaResponse.Done)
            {
                throw new Exception("Ollama API response not complete");
            }

            _logger.LogInformation("Ollama processing completed successfully for action: {Action}", request.Action);
            finalResult = ollamaResponse.Response;
        }

        await LogQuery(request, finalResult);
        return finalResult;
    }
}

public class AIOptions
{
    public string Endpoint { get; set; } = "http://localhost:11434/api/generate";
    public string Model { get; set; } = "llama3";
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
}
