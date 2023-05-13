using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zdk.Utilities.Base;

public abstract class DTORequestBase
{
}

public abstract class DTOResponseBase
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public IList<string>? Errors { get; set; }
}
