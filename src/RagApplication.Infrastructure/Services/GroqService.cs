
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RagApplication.Application.Interfaces;

namespace RagApplication.Infrastructure.Services;

public class GroqService : ILlmService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public GroqService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<string> GenerateResponseAsync(
        string question,
        CancellationToken cancellationToken = default)
    {
        string apiKey = _configuration["Groq:ApiKey"]
            ?? throw new InvalidOperationException(
                "Groq API key is missing.");

        var client = _httpClientFactory.CreateClient("Groq");

        var requestBody = new
        {
            model = "openai/gpt-oss-20b",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You are a helpful AI assistant. Answer clearly."
                },
                new
                {
                    role = "user",
                    content = question
                }
            },
            temperature = 0.7,
            max_completion_tokens = 1024
        };

        string json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "chat/completions");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        using var response = await client.SendAsync(
            request,
            cancellationToken);

        string responseContent = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Groq API error: {(int)response.StatusCode} - {responseContent}");
        }

        using var document = JsonDocument.Parse(responseContent);

        string? answer = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return answer ?? string.Empty;
    }
}
