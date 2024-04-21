using Cronos;
using FolderBackupTest.Models;
using FolderBackupTest.Services.Interfaces;

namespace FolderBackupTest.Services;

public class FolderBackupSettings : IFolderBackupSettings
{
    private readonly CronExpression _cronExpression;
    private readonly BackupMaker _backupMaker;

    public CronExpression CronExpression => _cronExpression;
    public BackupMaker BackupMaker => _backupMaker;
    
    
    
}