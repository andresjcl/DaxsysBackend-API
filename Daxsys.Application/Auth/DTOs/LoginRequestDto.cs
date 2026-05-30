using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Application.Auth.DTOs;

public class LoginRequestDto
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}