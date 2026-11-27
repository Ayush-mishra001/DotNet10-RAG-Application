using System;
using System.Collections.Generic;
using System.Text;

namespace RagApplication.Application.Interfaces
{
    internal interface ILlmService
    {
        Task<string> GenerateResponseAsync(
       string question,
       CancellationToken cancellationToken = default);
    }
}
