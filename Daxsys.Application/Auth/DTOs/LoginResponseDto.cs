using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Application.Auth.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserLoginDto User { get; set; } = null!;
}

public class UserLoginDto
{
    public string Id { get; set; } = null!;
    public bool IsAdmin { get; set; }
}
