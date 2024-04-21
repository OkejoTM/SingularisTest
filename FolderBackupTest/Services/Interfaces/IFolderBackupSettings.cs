using Cronos;
using FolderBackupTest.Models;

namespace FolderBackupTest.Services.Interfaces;

public interface IFolderBackupSettings
{
    public CronExpression CronExpression { get; }
    
}