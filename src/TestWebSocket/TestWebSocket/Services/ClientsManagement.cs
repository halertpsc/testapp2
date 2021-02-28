using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWebSocket.Models;

namespace TestWebSocket.Services
{
    public class ClientsManagement : IAsyncDisposable, IClientsManagement
    {
        private List<WebSocketConnection> _clients = new List<WebSocketConnection>();
        private object _locker = new object();
        public void AppendClient(WebSocket webSocket, TaskCompletionSource<object> completionSource)
        {
            var webSocketConnection = new WebSocketConnection(webSocket, completionSource);
            lock (_locker)
            {
                _clients.Add(webSocketConnection);
            }
        }


        public async Task NotifyClients(string message)
        {
            List<WebSocketConnection> clients;
            lock (_locker)
            {
                 clients = _clients.ToList();
            }

            if (clients != null)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                foreach (var client in clients)
                {
                    if (client.WebScoket.State == WebSocketState.Open)
                    {
                        await client.WebScoket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                        var incomingMessage = new byte[2];
                        var cancellationTokenSource = new CancellationTokenSource();
                        var receiveTask = client.WebScoket.ReceiveAsync(new ArraySegment<byte>(incomingMessage), cancellationTokenSource.Token);
                        var timeoutTask = Task.Delay(5000, cancellationTokenSource.Token);
                        await Task.WhenAny(new[] { receiveTask, timeoutTask });
                        cancellationTokenSource.Cancel();
                        if (receiveTask.IsCompleted &&  Encoding.UTF8.GetString(incomingMessage) == "ok")
                        {
                            continue;
                        }
                        await client.WebScoket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
                        client.WebScoket.Dispose();
                        client.TaskCompletionSource.SetResult(new object());
                        lock (_locker)
                        {
                            _clients.Remove(client);
                        }
                    }
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_clients != null)
            {
                foreach (var client in _clients)
                {
                    if (client.WebScoket.State == WebSocketState.Open)
                    {
                        await client.WebScoket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application is terminating", CancellationToken.None);
                    }
                    client.WebScoket.Dispose();
                    client.TaskCompletionSource.SetResult(new object());
                }
            }
        }
    }
}
