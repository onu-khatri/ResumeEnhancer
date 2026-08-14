using System;
using System.Collections.Generic;
using System.Text;

namespace AuthModuleAM.ResponsesDTO;

public class RegisterResponseDTO
{
    public string Message { get; set; } = "User Registered Successfully";
    public string Status { get; set; } = "Success";
    public int StatusCode { get; set; } = 200;
}
