using FolderBackupTest.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FolderBackupTest.Services;

public class FolderBackupService : IHostedService, IDisposable
{
    private readonly IFolderBackupSettings _settings;
    private readonly ILogger<FolderBackupService> _logger;

    private Timer? _timer;
    
    public FolderBackupService(IFolderBackupSettings settings, ILogger<FolderBackupService> logger)
    {
        _settings = settings;
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(
            (e) => ProcessTask(),
            null,
            TimeSpan.Zero, 
            TimeSpan.FromSeconds(1));
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        
        return Task.CompletedTask;
    }

    private void ProcessTask()
    {
        
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}