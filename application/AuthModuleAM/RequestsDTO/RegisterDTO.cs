using System;
using System.Collections.Generic;
using System.Text;

namespace AuthModuleAM.RequestsDTO;

public class RegisterDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public bool TwoStepVerification { get; set; } = false;
}
