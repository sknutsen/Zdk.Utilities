using Microsoft.AspNetCore.SignalR;

namespace Zdk.Utilities.Authentication.Data;

public class NameUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.Identity?.Name;
    }
}
