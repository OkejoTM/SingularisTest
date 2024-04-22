namespace FolderBackupTest.Models;

public class ErrorMessage
{
    public const string SectionSettingsNotFoundError = "Раздел FolderBackup не найден в файле конфигурации";
    public const string KeyNotFoundError = "Не найдено значение в конфигурационном файле: ";
    public const string CronExpressionNotFoundError = "Cron-выражение не указано";
    public const string FolderDoesntExistError = "Указанная папка не существует";
    public const string PathNotFoundError = "Путь к папке не указан";
    public const string CronParseError = "Cron-выражение задано в неверном формате";
    public const string ObjectDisposedError = "Неуправляемые ресурсы приложения уже подчищены";
    public const string DefaultMessage = "Невозможно запустить сервис";
    public const string UndefinedError = "Неизвестная ошибка";
}