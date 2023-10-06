using System.Windows;
using System.Windows.Controls;
using CsSolutionManager.UI.ViewModels;
using CsSolutionManger.Console.Models;

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
    }

    private void BtnBrowserSolution_Click(object sender, RoutedEventArgs e) 
        => _mainWindowViewModel.BrowseForSolution();

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => 
        _mainWindowViewModel.ProjectSelectionChanged((sender as ComboBox)?.SelectedItem as Project);

    private void BtnRight_Click(object sender, RoutedEventArgs e)
    {
        //Get selected nuget package
        //Convert to project
        _mainWindowViewModel.MoveNugetPackageToProject(DgNugetPackages.SelectedItem as NugetPackage);
    }

    private void BtLeft_Click(object sender, RoutedEventArgs e)
    {
        //Get project
        //Convert to nuget
    }
}