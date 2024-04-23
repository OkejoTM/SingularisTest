namespace FolderBackupTest.Services.Interfaces;

public interface IBackupMaker
{
    public string? SourcePath { get; set; }
    public string? DestinationPath { get; set; }

    public void DoBackup(CancellationToken cancellationToken);
}