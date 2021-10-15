using Microsoft.AspNetCore.SignalR;

namespace UploadProgressWithSignalR.Core
{
    public class NotifyHub:Hub
    {
        public async Task UpdatePrograss(int progress, string connectionId)
        => await Clients.Client(connectionId).SendAsync("updateProgress", progress);
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

    }
}
