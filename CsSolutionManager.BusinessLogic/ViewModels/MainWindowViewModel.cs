using System.ComponentModel;
using CsSolutionManager.BusinessLogic.Interfaces;
using CsSolutionManager.BusinessLogic.Services;
using DotNet.Cli.CommandLineInterfaces;
using DotNet.Cli.VisualStudio;

namespace CsSolutionManager.BusinessLogic.ViewModels
{
    public class MainWindowViewModel : IMainWindowViewModel, INotifyPropertyChanged
    {
        private readonly IOpenFileDialog _openFileDialog;
        private readonly IReferenceManagementService _referenceManagementService;
        private readonly DotNetCommandLineInterface _dotNetCli;
        private ISolution? _solution;
        private static string CodeRepositoryFolder => $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";

        public MainWindowViewModel(IOpenFileDialog openFileDialog, IReferenceManagementService referenceManagementService)
        {
            _openFileDialog = openFileDialog;
            _referenceManagementService = referenceManagementService;
            _dotNetCli = new DotNetCommandLineInterface();
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
                    OpenFileDialogFilter.SolutionFileFilter,
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

        public async Task MoveNugetPackageToProject(NugetPackage? selectedItem) => 
            await _referenceManagementService.ChangeToProjectReference(selectedItem, SelectedProject, _solution);

        public async Task MoveProjectToNugetPackage(Project selectedProject) =>
            await _referenceManagementService.ChangeToNugetReference(selectedProject, null, _solution);

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
        Task MoveProjectToNugetPackage(Project selectedProject);
    }
}