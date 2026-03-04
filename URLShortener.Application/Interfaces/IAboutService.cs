using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Domain.Models;

namespace URLShortener.Application.Interfaces
{
    public interface IAboutService
    {
        Task CreateEmptyAboutAsync();
        Task<string?> GetAboutContentAsync();
        Task UpdateAboutContentAsync(string content);
    }
}
