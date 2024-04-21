using FolderBackupTest.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FolderBackupTest.Services;

public class BackupMaker : IBackupMaker
{
    private readonly ILogger<BackupMaker> _logger;

    public string? SourcePath { get; set; }
    public string? DestinationPath { get; set; }

    public BackupMaker(ILogger<BackupMaker> logger)
    {
        _logger = logger;
    }

    public void DoBackup()
    {
        // Составить список файлов в source
        // Если base нет, сделать 1 копирование copy("base")
        // Если base есть, сделать инк копирование copy("inc_......")
    }

    private void Copy(string folderName)
    {
        // 
    }
    
    
    
    
}