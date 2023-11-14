using System.Windows;
using System.Windows.Controls;
using CsSolutionManager.BusinessLogic.ViewModels;
using DotNet.Cli.VisualStudio;

namespace CsSolutionManager.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IMainWindowViewModel _mainWindowViewModel;

    public MainWindow(IMainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        InitializeComponent();

        DataContext = _mainWindowViewModel;

        Loaded += async delegate (object sender, RoutedEventArgs e)
        {
            await _mainWindowViewModel.OnWindowReady(sender, e);
        };
    }

    private void BtnBrowserSolution_Click(object sender, RoutedEventArgs e) 
        => _mainWindowViewModel.BrowseForSolution();

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e) 
        => _mainWindowViewModel.ProjectSelectionChanged((sender as ComboBox)?.SelectedItem as Project);

    private void BtnRight_Click(object sender, RoutedEventArgs e) 
        => _mainWindowViewModel.MoveNugetPackageToProject(DgNugetPackages.SelectedItem as NugetPackage);

    private void BtLeft_Click(object sender, RoutedEventArgs e) 
        => _mainWindowViewModel.MoveProjectToNugetPackage(DgProjects.SelectedItem as Project);

    private void BtnPublish_Click(object sender, RoutedEventArgs e)
        => _mainWindowViewModel.PublishNugetPackage(DgNugetPackages.SelectedItem as NugetPackage);
}