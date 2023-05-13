using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zdk.Utilities.SignalR;

public abstract partial class BaseHub<HubName> : Hub
{
    protected readonly ILogger<HubName> logger;

    public BaseHub(IServiceProvider services)
    {
        logger = services.GetRequiredService<ILogger<HubName>>();
    }
}
