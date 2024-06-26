using FolderBackupTest.Models;
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

    public FolderBackupService(IFolderBackupSettings settings, ILogger<FolderBackupService> logger,
        IBackupMaker backupMaker)
    {
        _settings = settings;
        _logger = logger;
        _backupMaker = backupMaker;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Запуск FolderBackup...");
            DateTime? interval = _settings.CronExpression.GetNextOccurrence(DateTime.UtcNow);
            var seconds = (interval - DateTime.UtcNow)?.TotalSeconds ?? 0;

            _timer = new Timer(
                (e) => ProcessTask(cancellationToken),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(seconds));
        }
        catch (Exception)
        {
            _logger.LogError(ErrorMessage.DefaultMessage);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Остановка FolderBackup...");

            _timer?.Change(Timeout.Infinite, 0);
            Dispose();

            _logger.LogInformation("FolderBackup остановлен.");
        }
        catch (ObjectDisposedException)
        {
            _logger.LogError(ErrorMessage.ObjectDisposedError);
        }
        catch (Exception e)
        {
            _logger.LogCritical(ErrorMessage.UndefinedError);
            _logger.LogCritical(e.Message);
        }
        
        return Task.CompletedTask;
    }

    private void ProcessTask(CancellationToken cancellationToken)
    {
        if (Monitor.TryEnter(_rootSync))
        {
            // Start
            _logger.LogInformation("Запуск очередной задачи FolderBackup...");

            _backupMaker.DoBackup(cancellationToken);

            _logger.LogInformation("FolderBackup отработал");
            // End task
            Monitor.Exit(_rootSync);
        }
        else
        {
            // Skip Task
            _logger.LogInformation("FolderBackup еще не знакончил работу. Пропуск");
        }
    }


    public void Dispose()
    {
        _timer?.Dispose();
    }
}