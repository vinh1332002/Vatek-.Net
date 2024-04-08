using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class DocumentPageHub : Hub
    {
        public async Task SendDocumentPageUpdate(string message)
        {
            await Clients.All.SendAsync("refreshDocumentPage", message);
        }
    }
}
