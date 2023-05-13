using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zdk.Utilities.Base;

namespace Zdk.Utilities.Authentication;

public class DTOZdkRegisterRequest : DTORequestBase
{
    public string? ReturnUrl { get; set; }

    [Required]
    public string Username { get; set; }

    public string? Email { get; set; }

    [Required]
    public string Password { get; set; }
}

public class DTOZdkRegisterResponse : DTOResponseBase
{
    public string? ReturnUrl { get; set; }
}
