using System.Configuration;
using FolderBackupTest.Models;
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
    .ConfigureServices((hostingContext, services) =>
    {
        // Add services.
        services.Configure<JsonSettings>(hostingContext.Configuration.GetSection("FolderBackup"));
        services.AddHostedService<FolderBackupService>();
        services.AddSingleton<IFolderBackupSettings, FolderBackupSettings>();
        services.AddSingleton<IBackupMaker, BackupMaker>();
        services.AddLogging();
    });

await builder.RunConsoleAsync();