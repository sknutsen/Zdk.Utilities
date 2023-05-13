using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zdk.Utilities.Base;

namespace Zdk.Utilities.Authentication;

public class DTOZdkLoginRequest : DTORequestBase
{
    public string? ReturnUrl { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}

public class DTOZdkLoginResponse : DTOResponseBase
{
    public string? ReturnUrl { get; set; }
}
