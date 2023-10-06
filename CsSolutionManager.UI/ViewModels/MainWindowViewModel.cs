using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using CsSolutionManger.Console;
using CsSolutionManger.Console.DotNetCli;
using CsSolutionManger.Console.Interfaces;
using CsSolutionManger.Console.Models;

namespace CsSolutionManager.UI.ViewModels
{
    public class MainWindowViewModel : IMainWindowViewModel, INotifyPropertyChanged
    {
        private readonly IOpenFileDialog _openFileDialog;
        private readonly IReferenceManagementService _referenceManagementService;
        private readonly DotNetCli _dotNetCli;
        private ISolution? _solution;

        private static string CodeRepositoryFolder => $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";

        public MainWindowViewModel(IOpenFileDialog openFileDialog, IReferenceManagementService referenceManagementService)
        {
            _openFileDialog = openFileDialog;
            _referenceManagementService = referenceManagementService;
            _dotNetCli = new DotNetCli();
        }

        public string SolutionPath { get; set; } = string.Empty;
        public List<Project> Projects { get; set; } = new ();
        public List<NugetPackage> NugetPackages { get; set; } = new();
        public List<Project> ProjectReferences { get; set; } = new();
        public Project? SelectedProject { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task BrowseForSolution()
        {
            if (!(_openFileDialog.ShowDialog(
                    OpenFileDialogWrapper.SolutionFileFilter,
                    CodeRepositoryFolder,
                    "Please select a solution file.") ?? false)) 
                return;

            await InitialiseSolutionData();
        }

        public async Task ProjectSelectionChanged(Project? selectedProject)
        {
            if(selectedProject == null) 
                return;

            SelectedProject = selectedProject;
            NugetPackages = await selectedProject.Packages;
            ProjectReferences = await selectedProject.Projects;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NugetPackages)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProjectReferences)));
        }

        public async Task MoveNugetPackageToProject(NugetPackage? selectedItem)
        {
            if(selectedItem is null || SelectedProject is null || _solution is null)
                return;

            if (selectedItem.RegisteredProject is null)
                RegisterProjectFileWithNugetPackage(selectedItem);

            if(selectedItem.RegisteredProject is null)
                return;

            await SelectedProject.RemovePackage(selectedItem);
            await SelectedProject.AddProject(selectedItem.RegisteredProject, _solution);
        }

        private void RegisterProjectFileWithNugetPackage(NugetPackage package)
        {
            if(_solution is null) 
                return;

            MessageBox.Show("Please register a csproj file with this nuget package before continuing");

            _openFileDialog.InitialDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";
            _openFileDialog.Title = "Please select a csproj file.";

            if (!_openFileDialog.ShowDialog(
                    OpenFileDialogWrapper.CsprojFileFilter,
                    CodeRepositoryFolder,
                    "Please select a csproj file.") ?? false)
                return;

            var project = new Project(_openFileDialog.FileName, _dotNetCli.Solution(_solution).Projects);
            package.RegisterWithProject(project);
        }

        private async Task InitialiseSolutionData()
        {
            SolutionPath = _openFileDialog.FileName;
            _solution = new Solution(SolutionPath, _dotNetCli);
            Projects = await _solution.Projects;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SolutionPath)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Projects)));
        }
    }

    public interface IMainWindowViewModel
    {
        string SolutionPath { get; set; }
        Task BrowseForSolution();
        Task ProjectSelectionChanged(Project? selectedProject);
        Task MoveNugetPackageToProject(NugetPackage? selectedItem);
    }
}