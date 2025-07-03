using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerPlatform.Core.DTOs;

namespace HackerPlatform.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto dto);
        Task<string> RegisterAsync(RegisterDto dto);
    }
}
