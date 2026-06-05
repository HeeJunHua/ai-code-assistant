using Microsoft.AspNetCore.Mvc;

namespace AICodeAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MockProviderController : ControllerBase
{
    // Simple mock that returns an OpenAI‑style JSON response.
    // The request body is ignored – we just prove the back‑end can call any
    // custom endpoint and correctly parse the result.
    [HttpPost("chat")]
    public IActionResult Chat([FromBody] object _)
    {
        var response = new
        {
            id = "mock-1",
            @object = "chat.completion",
            created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            model = "mock-model",
            choices = new[]
            {
                new {
                    index = 0,
                    message = new { role = "assistant", content = "Mocked response: OK" },
                    finish_reason = "stop"
                }
            },
            usage = new { prompt_tokens = 10, completion_tokens = 5, total_tokens = 15 }
        };
        return Ok(response);
    }
}
