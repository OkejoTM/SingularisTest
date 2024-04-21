using FolderBackupTest.Services;
using FolderBackupTest.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((services) =>
    {
        // Add services.
        services.AddHostedService<FolderBackupService>();
        services.AddSingleton<IFolderBackupSettings, FolderBackupSettings>();
        services.AddSingleton<IBackupMaker, BackupMaker>();
        services.AddLogging();
    });

await builder.RunConsoleAsync();