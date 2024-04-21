using Cronos;
using FolderBackupTest.Models;
using FolderBackupTest.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FolderBackupTest.Services;

public class FolderBackupSettings : IFolderBackupSettings
{
    private readonly ILogger<FolderBackupSettings> _logger;
    private readonly BackupMaker _backupMaker;

    private CronExpression _cronExpression;
    private JsonSettings? _jsonSettings;
    
    public CronExpression CronExpression => _cronExpression;
    public BackupMaker BackupMaker => _backupMaker;

    public FolderBackupSettings(IConfiguration configuration, ILogger<FolderBackupSettings> logger)
    {
        // read from config file
        _jsonSettings = configuration.GetSection("FolderBackup").Get<JsonSettings>();
        _backupMaker = new BackupMaker();
        
        _logger = logger;
        
        SetSettings();
        

    }

    private void SetSettings()
    {
        _backupMaker.SourcePath = _jsonSettings!.SourcePath!;
        _backupMaker.DestinationPath = _jsonSettings!.DestinationPath!;
        _cronExpression = CronExpression.Parse(_jsonSettings?.CronExpression);
    }
    
}