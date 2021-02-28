using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebSocket.Models;

namespace TestWebSocket.Services
{
    public class ProcessInfoStorage : IProcessInfoStorage
    {
        private volatile IEnumerable<ProcessInfo> _currentProcessInfo = new List<ProcessInfo>();

        public void SetProcessInfo(IEnumerable<ProcessInfo> processInfo)
        {
            _currentProcessInfo = processInfo;
        }

        public IEnumerable<ProcessInfo> ProcessInfo => _currentProcessInfo;
    }
}
