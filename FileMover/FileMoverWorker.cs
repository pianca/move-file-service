using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileMover
{
    public class FileMoverWorker : BackgroundService
    {
        private readonly ILogger<FileMoverWorker> _logger;
        private readonly string _source;
        private readonly string _destination;
        private readonly int _interval;

        public FileMoverWorker(ILogger<FileMoverWorker> logger, IOptions<FileMoverOptions> options)
        {
            _logger = logger;
            _source = options.Value.Source;
            _destination = options.Value.Destination;
            _interval = options.Value.Interval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("_source: {_source}", _source);
                _logger.LogInformation("_destination: {_destination}", _destination);
                _logger.LogInformation("_interval: {_interval}", _destination);
                Move(_source, _destination);
                await Task.Delay(_interval * 1000, stoppingToken);
            }
        }

        private void Move(string source, string destination)
        {
            DirectoryInfo di = new DirectoryInfo(source);
            var files = di.EnumerateFiles("*.*", SearchOption.AllDirectories).ToList();
            files.ForEach(f => 
            {
                _logger.LogInformation("File: {f}", f.FullName);
                var newFilePath = Path.Combine(destination, f.Name);
                _logger.LogInformation("New File: {f}", newFilePath);
                f.MoveTo(newFilePath);
                var result = File.Exists(newFilePath);
                if(result)
                {

                    _logger.LogInformation("File moved from {s} to {d}", f.FullName, newFilePath);
                }
                else
                {
                    _logger.LogWarning("Failed to move file from {s} to {d}", f.FullName, newFilePath);
                }
            });

        }
    }
}
