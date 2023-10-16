using System.ComponentModel;
using CsSolutionManager.BusinessLogic.Interfaces;
using CsSolutionManager.BusinessLogic.Services;
using CsSolutionManager.DataLayer.Entities;
using DotNet.Cli.VisualStudio;
using NugetPackage = DotNet.Cli.VisualStudio.NugetPackage;
using Project = DotNet.Cli.VisualStudio.Project;

namespace CsSolutionManager.BusinessLogic.ViewModels;

public class MainWindowViewModel : IMainWindowViewModel, INotifyPropertyChanged
{
    private static string CodeRepositoryFolder => $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";

    private readonly IOpenFileDialog _openFileDialog;
    private readonly IReferenceManagementService _referenceManagementService;
    private readonly ISolutionService _solutionService;
    private readonly IApplicationHistoryService _applicationHistoryService;

    public MainWindowViewModel(
        IOpenFileDialog openFileDialog, 
        IReferenceManagementService referenceManagementService, 
        ISolutionService solutionService, 
        IApplicationHistoryService applicationHistoryService)
    {
        _openFileDialog = openFileDialog;
        _referenceManagementService = referenceManagementService;
        _solutionService = solutionService;
        _applicationHistoryService = applicationHistoryService;
    }

    public string SolutionPath { get; set; } = string.Empty;
    public List<Project> Projects { get; set; } = new ();
    public List<NugetPackage> NugetPackages { get; set; } = new();
    public List<Project> ProjectReferences { get; set; } = new();
    public Project? SelectedProject { get; set; }

    public async Task BrowseForSolution()
    {
        if (!(_openFileDialog.ShowDialog(
                OpenFileDialogFilter.SolutionFileFilter,
                CodeRepositoryFolder,
                "Please select a solution file.") ?? false)) 
            return;

        ISolution solution = await SetSolution(_openFileDialog.FileName);
        _applicationHistoryService.UpdateSolution(solution);
    }

    public async Task ProjectSelectionChanged(Project? selectedProject)
    {
        if(selectedProject == null) 
            return;

        SelectedProject = selectedProject;
        NugetPackages = await selectedProject.Packages;
        ProjectReferences = await selectedProject.Projects;

        InvokePropertyChangedEventArgs(nameof(NugetPackages), nameof(ProjectReferences));
    }

    private async Task<ISolution> SetSolution(string path)
    {
        _solutionService.Init(path);
        SolutionPath = _solutionService.CurrentSolution?.FullPath ?? string.Empty;
        Projects = await _solutionService.GetProjects();

        InvokePropertyChangedEventArgs(nameof(SolutionPath), nameof(Projects));

        if (_solutionService.CurrentSolution is null)
            throw new Exception("Solution cannot be null");

        return _solutionService.CurrentSolution;
    }

    #region Events

    public async Task MoveNugetPackageToProject(NugetPackage? selectedItem) =>
        await _referenceManagementService.ChangeToProjectReference(selectedItem, SelectedProject, _solutionService.CurrentSolution);

    public async Task MoveProjectToNugetPackage(Project? selectedProject) =>
        await _referenceManagementService.ChangeToNugetReference(selectedProject, null, _solutionService.CurrentSolution);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void InvokePropertyChangedEventArgs(params string[] propertyNames)
        => propertyNames
            .ToList()
            .ForEach(propertyName => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));

    public async Task OnWindowReady(object sender, object routedEventArgs)
    {
        ApplicationHistory? latestVersion = _applicationHistoryService.GetLatestHistory();
        if(latestVersion is null)
            return;

        await SetSolution(latestVersion.Solution.FullPath);
    }

    #endregion
}

public interface IMainWindowViewModel
{
    string SolutionPath { get; set; }
    Task BrowseForSolution();
    Task ProjectSelectionChanged(Project? selectedProject);
    Task MoveNugetPackageToProject(NugetPackage? selectedItem);
    Task MoveProjectToNugetPackage(Project? selectedProject);
    Task OnWindowReady(object sender, object routedEventArgs);
}