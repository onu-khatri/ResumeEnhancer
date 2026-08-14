using System;
using System.Collections.Generic;
using System.Text;

namespace AuthModuleAM.ResponsesDTO;

public class LoginResponseDTO
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "Login Successfully";
}
