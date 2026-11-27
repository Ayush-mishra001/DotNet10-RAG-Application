using Microsoft.Extensions.DependencyInjection;
using RagApplication.Application.Interfaces;
using RagApplication.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RagApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
        {
            services.AddHttpClient("Groq", client =>
            {
                client.BaseAddress = new Uri(
                    "https://api.groq.com/openai/v1/");

            });

            services.AddScoped<ILlmService, GroqService>();

            return services;
        }
    }
}
