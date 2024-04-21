namespace FolderBackupTest.Models;

public class JsonSettings
{
    public string? SourcePath { get; set; }
    public string? DestinationPath { get; set; }
    public string? CronExpression { get; set; }
}