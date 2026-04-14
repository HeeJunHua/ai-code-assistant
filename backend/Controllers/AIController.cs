using AICodeAssistant.Api.Models;
using AICodeAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AICodeAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly ILogger<AIController> _logger;

    public AIController(IAIService aiService, ILogger<AIController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessCode([FromBody] ProcessRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest(new { error = "Code is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Action))
            {
                return BadRequest(new { error = "Action is required" });
            }

            // Validate action type
            var validActions = new[] { "explain", "fix", "optimize" };
            if (!validActions.Contains(request.Action.ToLower()))
            {
                return BadRequest(new { error = $"Invalid action. Please use one of: {string.Join(", ", validActions)}" });
            }

            _logger.LogInformation("Processing code with action: {Action}", request.Action);

            // Process code with AI service
            string result = await _aiService.ProcessCodeAsync(request.Code, request.Action);

            var response = new ProcessResponse
            {
                Result = result,
                Action = request.Action,
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            _logger.LogInformation("Successfully processed code with action: {Action}", request.Action);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided for code processing");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing code with AI");
            return StatusCode(500, new { 
                error = "An error occurred while processing your request", 
                details = ex.Message 
            });
        }
    }
}