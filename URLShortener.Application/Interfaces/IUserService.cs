using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Domain.DTOs;

namespace URLShortener.Application.Interfaces
{
    public interface IUserService
    {
        Task<string?> LoginAsync(AuthenticationRequest request);
        Task RegisterAsync(AuthenticationRequest request);
    }
}
