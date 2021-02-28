using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebSocket.Models
{
    public class ProcessInfo
    {
        public ProcessInfo(string name, double cpuUsage, double memoryUsage)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CpuUsage = cpuUsage;
            MemoryUsage = memoryUsage;
        }

        public string Name { get;  }
        public double CpuUsage { get; }
        public double MemoryUsage { get;  }
    }
}
