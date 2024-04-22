using Cronos;
using FolderBackupTest.Models;

namespace FolderBackupTest.Services.Interfaces;

public interface IFolderBackupSettings : IDisposable
{
    public CronExpression CronExpression { get; }
    
}