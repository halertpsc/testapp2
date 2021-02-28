using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace TestWebSocket.Models
{
    public class WebSocketConnection
    {
        public WebSocketConnection(WebSocket webScoket, TaskCompletionSource<object> taskCompletionSource)
        {
            WebScoket = webScoket ?? throw new ArgumentNullException(nameof(webScoket));
            TaskCompletionSource = taskCompletionSource ?? throw new ArgumentNullException(nameof(taskCompletionSource));
        }

       public  WebSocket WebScoket { get;}

       public TaskCompletionSource<object> TaskCompletionSource { get; }
    }
}
