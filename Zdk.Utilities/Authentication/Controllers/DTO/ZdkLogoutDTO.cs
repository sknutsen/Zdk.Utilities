using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zdk.Utilities.Base;

namespace Zdk.Utilities.Authentication;

public class DTOZdkLogoutRequest : DTORequestBase
{
    public string? ReturnUrl { get; set; }
}

public class DTOZdkLogoutResponse : DTOResponseBase
{
    public string? ReturnUrl { get; set; }
}
