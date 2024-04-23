using System.Configuration;
using Cronos;
using FolderBackupTest.Models;
using FolderBackupTest.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace FolderBackupTest.Services;

public class FolderBackupSettings : IFolderBackupSettings
{
    private readonly ILogger<FolderBackupSettings> _logger;
    private readonly IBackupMaker _backupMaker;


    private CronExpression _cronExpression;
    private JsonSettings? _jsonSettings;
    private readonly IDisposable _changeToken;

    public CronExpression CronExpression => _cronExpression;

    public FolderBackupSettings(IConfiguration configuration, ILogger<FolderBackupSettings> logger,
        IBackupMaker backupMaker)
    {
        // read from config file
        _jsonSettings = configuration.GetSection("FolderBackup").Get<JsonSettings>();
        _backupMaker = backupMaker;

        _logger = logger;

        SetSettings();

        _changeToken = ChangeToken.OnChange(
            configuration.GetReloadToken,
            () =>
            {
                _logger.LogInformation("Файл конфигурации был изменен, загрузка нового конфигурационного файла");
                _jsonSettings = configuration.GetSection("FolderBackup").Get<JsonSettings>();
                SetSettings();
            });
    }

    private void SetSettings()
    {
        try
        {
            CheckConfigurations();
            _backupMaker.SourcePath = _jsonSettings!.SourcePath!.TrimEnd('\\', '/');
            _backupMaker.DestinationPath = _jsonSettings!.DestinationPath!.TrimEnd('\\', '/');
            _cronExpression = CronExpression.Parse(_jsonSettings?.CronExpression);
        }
        catch (ConfigurationErrorsException)
        {
            _logger.LogError(ErrorMessage.SectionSettingsNotFoundError);
        }
        catch (DirectoryNotFoundException)
        {
            _logger.LogError(ErrorMessage.FolderDoesntExistError);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError($"{ErrorMessage.KeyNotFoundError} {ex.Message}");
        }
        catch (CronFormatException)
        {
            _logger.LogError(ErrorMessage.CronParseError);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ErrorMessage.UndefinedError);
            _logger.LogCritical(ex.Message);
        }
    }

    private void CheckConfigurations()
    {
        if (_jsonSettings == null)
        {
            throw new ConfigurationErrorsException();
        }

        if (_jsonSettings.SourcePath == null)
        {
            throw new KeyNotFoundException(ErrorMessage.PathNotFoundError);
        }

        if (_jsonSettings.DestinationPath == null)
        {
            throw new KeyNotFoundException(ErrorMessage.PathNotFoundError);
        }

        if (_jsonSettings.CronExpression == null)
        {
            throw new KeyNotFoundException(ErrorMessage.CronExpressionNotFoundError);
        }

        if (!Directory.Exists(_jsonSettings!.SourcePath))
        {
            throw new DirectoryNotFoundException();
        }

        if (!Directory.Exists(_jsonSettings!.DestinationPath))
        {
            throw new DirectoryNotFoundException();
        }
    }

    public void Dispose()
    {
        _changeToken.Dispose();
    }
}