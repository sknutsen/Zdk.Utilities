using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zdk.Utilities.SignalR;

public abstract partial class BaseHub<HubName>
{
    public async Task JoinGroup()
    {
        int groupId = await GetGroupId();

        await JoinGroup(groupId);
    }

    protected async Task JoinGroup(int groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
    }

    protected async Task LeaveGroup()
    {
        int groupId = await GetGroupId();

        await LeaveGroup(groupId);
    }

    public async Task LeaveGroup(int groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
    }
}
