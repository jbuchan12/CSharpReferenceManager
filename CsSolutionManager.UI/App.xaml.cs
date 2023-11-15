using System.Windows;
using CsSolutionManager.BusinessLogic.Interfaces;
using CsSolutionManager.BusinessLogic.Services;
using CsSolutionManager.BusinessLogic.ViewModels;
using CsSolutionManager.BusinessLogic.Wrappers;
using CsSolutionManager.DataLayer;
using CsSolutionManager.DataLayer.Entities;
using CsSolutionManager.DataLayer.Repositories;
using CsSolutionManager.UI.UIWrappers;
using DotNet.Cli.CommandLineInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CsSolutionManager.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        IHostBuilder builder = CreateHostBuilder();
        IHost host = builder.Build();

        var mainWindow = host.Services.GetService<MainWindow>();
        mainWindow?.Show();
    }

    private static IHostBuilder CreateHostBuilder()
        => Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddLogging(l =>
                {
                    l.SetMinimumLevel(LogLevel.Trace);
                    l.AddNLog();
                });

                //Wrappers
                services.AddSingleton<IOpenFileDialog, OpenFileDialogWrapper>();
                services.AddSingleton<IMessageBox, MessageBoxWrapper>();
                services.AddSingleton<IFileSystem, FileSystem>();
                services.AddSingleton<IPublishCommandLine, PublishCommandLine>();

                //WPF Windows
                services.AddTransient(typeof(MainWindow));

                //View Models
                services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();

                //Services
                services.AddSingleton<IApplicationService, ApplicationService>();
                services.AddSingleton<IReferenceManagementService, ReferenceManagementService>();
                services.AddSingleton<ISolutionService, SolutionService>();
                services.AddSingleton<IApplicationHistoryService, ApplicationHistoryService>();
                services.AddSingleton<IMapperService, MapperService>();
                services.AddSingleton<INugetPublishService, NugetPublishService>();

                //EF
                services.AddSingleton<ICsSolutionManagerContext, CsSolutionManagerContext>();

                //EF Repos
                services.AddSingleton<IApplicationHistoryRepository, ApplicationHistoryRepository>();
                services.AddSingleton<ISolutionRepository, SolutionRepository>();
                services.AddSingleton<IGenericRepository<ApplicationHistory>, GenericRepository<ApplicationHistory>>();
                services.AddSingleton<INugetPackageRepository, NugetPackageRepository>();
                services.AddSingleton<IProjectRepository, ProjectRepository>();
            });
}