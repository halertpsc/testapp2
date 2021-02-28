using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebSocket.Models;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace TestWebSocket.Services
{
    public class ProcessInfoService : BackgroundService
    {
       
        private readonly IClientsManagement _clientsManagement;
        private readonly IProcessInfoStorage _processInfoStorage;
        private readonly ILogger _logger;
        private const double _cpuTreshold = 5;
        private const double _memoryTreshold = 1000000000;
       

        public ProcessInfoService(IClientsManagement clientsManagement, IProcessInfoStorage processInfoStorage, ILogger<ProcessInfoService> logger)
        {
            _processInfoStorage = processInfoStorage;
            _clientsManagement = clientsManagement;
            _logger = logger;
        }

   
        private async Task<IEnumerable<ProcessInfo>> GetProcessInfoInternal()
        {
            var processes = System.Diagnostics.Process.GetProcesses().Where(process => !process.ProcessName.Equals("Idle", StringComparison.InvariantCultureIgnoreCase));
            var tasks = processes.Select(async process => new ProcessInfo(process.ProcessName, await GetCpuUsage(process, TimeSpan.FromSeconds(1)), process.WorkingSet64)).ToList();
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex, "Some processes were terminated during measurement");
            }
            return tasks.Where(task => task.IsCompletedSuccessfully).Select(task => task.Result).OrderByDescending(process => process.CpuUsage);
        }

        private async Task<double> GetCpuUsage(System.Diagnostics.Process process, TimeSpan onPeriod)
        {
            var startTime = DateTimeOffset.UtcNow;
            var startCpuUsage = process.TotalProcessorTime;
            await Task.Delay(onPeriod);
            var endTIme = DateTimeOffset.UtcNow;
            var endCpuUsage = process.TotalProcessorTime;
            var cpuUsed = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var timePassed = (endTIme - startTime).TotalMilliseconds;
            return cpuUsed / (Environment.ProcessorCount * timePassed) * 100;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _processInfoStorage.SetProcessInfo( await GetProcessInfoInternal());
                    var overTReshhold = _processInfoStorage.ProcessInfo.Where(processInfo => processInfo.MemoryUsage >= _memoryTreshold || processInfo.CpuUsage >= _cpuTreshold);
                    if (overTReshhold.Any())
                    {
                        await _clientsManagement.NotifyClients(JsonConvert.SerializeObject(overTReshhold));
                    }
                }

                catch (Exception ex)
                {
                    _logger.LogError($"{JsonConvert.SerializeObject(ex)}");
                }
            }
        }
    }
}
