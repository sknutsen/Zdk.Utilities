using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Zdk.Utilities.Authentication.ViewModels.Authorization;

public class LogoutViewModel
{
    [BindNever]
    public string RequestId { get; set; }
}
