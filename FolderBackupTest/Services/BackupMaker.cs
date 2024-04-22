using System.Security.Cryptography;
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
        if (SourcePath == null || DestinationPath == null)
        {
            _logger.LogError("Резервное копирование в эти пути не возможно");
            return;
        }
        
        var baseDir = Path.Combine(DestinationPath, "base");
        // Проверяем наличие base
        if (!Directory.Exists(baseDir))
        {
            _logger.LogInformation("Начало создания base папки");
            // Создаем base
            CreateBaseCopy();
            _logger.LogInformation("base Папка создана успешно");
        }
        else
        {
            // Иначе, создаем инкрементальную копию
            List<string> filesToCopy = FindChangedAndCreatedFiles(SourcePath);
            if (filesToCopy.Count != 0)
            {
                string incrementalDirectoryName = "inc_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
                string incrementalDirectoryPath = Path.Combine(DestinationPath, incrementalDirectoryName);
                _logger.LogInformation($"Создание папки {incrementalDirectoryName}");
                Directory.CreateDirectory(incrementalDirectoryPath);
                foreach (var file in filesToCopy)
                {
                    CopyFile(file, Path.Combine(incrementalDirectoryPath, file.Substring(SourcePath.Length + 1)));
                }
                _logger.LogInformation($"Папка {incrementalDirectoryName} создана успешно");
            }
            else
            {
                _logger.LogInformation("Файлы не были изменены или добавлены. Копирование не происходит");
            }
        }
    }
    
    
    private List<string> FindChangedAndCreatedFiles(string source)
    {
        // Получить все файлы в source
        _logger.LogInformation("Начало подсчета файлов для копирования");
        var items = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        List<string> filesToCopy = new List<string>();
        foreach (var item in items)
        {
            var filePath = item.Substring(SourcePath .Length + 1);
            if (CheckFileOnModified(filePath))
            {
                filesToCopy.Add(item);
                _logger.LogInformation($"{item} необходимо скопировать");
            }
        }

        _logger.LogInformation($"{filesToCopy.Count} файлов для копирования");
        return filesToCopy;
    }

    private bool CheckFileOnModified(string filePath)
    {
        bool needsToCopy = true;
        var directories = Directory.GetDirectories(DestinationPath);
        // Проверка по всем inc и base
        foreach (var dir in directories)
        {
            var destinationFilePath = Path.Combine(dir, filePath);
            var sourceFilePath = Path.Combine(SourcePath, filePath);
            if (File.Exists(destinationFilePath))
            {
                needsToCopy = IsModified(sourceFilePath, destinationFilePath);
            }
        }
        return needsToCopy;
    }
    
    private void CopyFile(string sourceFilePath, string destinationFilePath)
    {
        _logger.LogInformation($"Копирование файла {sourceFilePath}");
        var dirName = Path.GetDirectoryName(destinationFilePath); 
        if (dirName != null &&!Directory.Exists(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        File.Copy(sourceFilePath, destinationFilePath);
        _logger.LogInformation("Файл скопирован");
    }
    
    private void CreateBaseCopy()
    {
        string baseDirectory = Path.Combine(DestinationPath, "base");
        Directory.CreateDirectory(baseDirectory);

        // Копируем все файлы и папки из источника в папку base
        CopyAllFilesAndDirectories(SourcePath, baseDirectory);
    }

    private void CopyAllFilesAndDirectories(string source, string destination)
    {
        foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(source, destination));

        foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(source, destination), true);
    }

    private bool IsModified(string sourceFile, string destinationFile)
    {
        return !ComputeFileHash(sourceFile).Equals(ComputeFileHash(destinationFile));
    }
    
    private static string ComputeFileHash(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (MD5 sha256 = MD5.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(fs);
                return BitConverter.ToString(hashBytes);
            }
        }
    }
    
    
    
    
}