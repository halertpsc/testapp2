using System.Collections.Generic;
using TestWebSocket.Models;

namespace TestWebSocket.Services
{
    public interface IProcessInfoStorage
    {
        IEnumerable<ProcessInfo> ProcessInfo { get; }

        void SetProcessInfo(IEnumerable<ProcessInfo> processInfo);
    }
}