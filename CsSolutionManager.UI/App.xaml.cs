using System.Windows;
using CsSolutionManager.BusinessLogic.Interfaces;
using CsSolutionManager.BusinessLogic.Services;
using CsSolutionManager.BusinessLogic.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                services.AddSingleton<ICsSolutionManagerService, CsSolutionManagerService>();
                services.AddSingleton<IOpenFileDialog, OpenFileDialogWrapper>();
                services.AddSingleton<IReferenceManagementService, ReferenceManagementService>();

                //WPF Windows
                services.AddTransient(typeof(MainWindow));
                //View Models
                services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
            });
}