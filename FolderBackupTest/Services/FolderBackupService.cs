using FolderBackupTest.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FolderBackupTest.Services;

public class FolderBackupService : IHostedService, IDisposable
{
    private readonly IFolderBackupSettings _settings;
    private readonly ILogger<FolderBackupService> _logger;
    private readonly IBackupMaker _backupMaker;

    private Timer? _timer;

    private readonly object _rootSync = new object();
    
    public FolderBackupService(IFolderBackupSettings settings, ILogger<FolderBackupService> logger, IBackupMaker backupMaker)
    {
        _settings = settings;
        _logger = logger;
        _backupMaker = backupMaker;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        
        DateTime? interval = _settings.CronExpression.GetNextOccurrence(DateTime.UtcNow);
        var seconds = (interval - DateTime.UtcNow)?.TotalSeconds ?? 0;

        _timer = new Timer(
            (e) => ProcessTask(),
            null,
            TimeSpan.Zero, 
            TimeSpan.FromSeconds(seconds));
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        
        return Task.CompletedTask;
    }

    private void ProcessTask()
    {
        if (Monitor.TryEnter(_rootSync))
        {
            // Start
            
            _backupMaker.DoBackup();
            
            // End task
            Monitor.Exit(_rootSync);
        }
        else
        {
            // Skip Task
        }
    }

    
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}