using Zdk.Utilities.Authentication.Helpers;

namespace Zdk.Utilities.SignalR;

public abstract partial class BaseHub<HubName>
{
    protected abstract Task<int> GetGroupId();

    protected string GetUserId()
    {
        string userId = this.Context.User?.GetUserId() ?? "???";
        
        return userId;
    }
}
