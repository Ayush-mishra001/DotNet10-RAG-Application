
using Microsoft.AspNetCore.Mvc;
using RagApplication.Application.Interfaces;

namespace RagApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ILlmService _llmService;

    public ChatController(ILlmService llmService)
    {
        _llmService = llmService;
    }

    [HttpPost]
    public async Task<IActionResult> AskQuestion(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest("Question is required.");
        }

        string answer = await _llmService.GenerateResponseAsync(
            request.Question,
            cancellationToken);

        return Ok(new
        {
            Question = request.Question,
            Answer = answer
        });
    }
}

public class ChatRequest
{
    public string Question { get; set; } = string.Empty;
}
