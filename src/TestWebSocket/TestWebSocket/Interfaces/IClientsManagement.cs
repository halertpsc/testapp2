using System.Net.WebSockets;
using System.Threading.Tasks;

namespace TestWebSocket.Services
{
    public interface IClientsManagement
    {
        void AppendClient(WebSocket webSocket, TaskCompletionSource<object> completionSource);
        ValueTask DisposeAsync();
        Task NotifyClients(string message);
    }
}