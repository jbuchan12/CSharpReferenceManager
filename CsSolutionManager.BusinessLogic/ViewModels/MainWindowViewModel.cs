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
    private readonly INugetPublishService _nugetPublishService;

    public MainWindowViewModel(
        IOpenFileDialog openFileDialog, 
        IReferenceManagementService referenceManagementService, 
        ISolutionService solutionService, 
        IApplicationHistoryService applicationHistoryService,
        INugetPublishService nugetPublishService)
    {
        _openFileDialog = openFileDialog;
        _referenceManagementService = referenceManagementService;
        _solutionService = solutionService;
        _applicationHistoryService = applicationHistoryService;
        _nugetPublishService = nugetPublishService;
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

        ISolution? solution = await SetSolution(_openFileDialog.FileName);
        if(solution is null) 
            return;

        _applicationHistoryService.UpdateSolution(solution);
    }

    public async Task ProjectSelectionChanged(Project? selectedProject)
    {
        if(selectedProject is null) 
            return;

        SelectedProject = selectedProject;
        selectedProject.ProjectDataChanged += async (sender, args) =>
        {
            await RefreshGrids();
        };
        
        await RefreshGrids();
    }

    private async Task<ISolution?> SetSolution(string path)
    {
        try
        {
            _solutionService.Init(path);
        }
        catch (ArgumentException e) when(e.ParamName == "solutionFullPath")
        {
            return null;
        }

        SolutionPath = _solutionService.CurrentSolution?.FullPath ?? string.Empty;
        Projects = await _solutionService.GetProjects();

        InvokePropertyChangedEventArgs(nameof(SolutionPath), nameof(Projects));

        if (_solutionService.CurrentSolution is null)
            throw new Exception("Solution cannot be null");

        return _solutionService.CurrentSolution;
    }

    private async Task RefreshGrids()
    {
        if (SelectedProject is null)
            return;

        NugetPackages = await SelectedProject.Packages;
        ProjectReferences = await SelectedProject.Projects;

        InvokePropertyChangedEventArgs(nameof(NugetPackages), nameof(ProjectReferences));
    }

    #region Events

    public async Task MoveNugetPackageToProject(NugetPackage? selectedItem) =>
        await _referenceManagementService.ChangeToProjectReference(selectedItem, SelectedProject, _solutionService.CurrentSolution);

    public async Task MoveProjectToNugetPackage(Project? referencedProject) =>
        await _referenceManagementService.ChangeToNugetReference(SelectedProject, referencedProject, _solutionService.CurrentSolution);

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task OnWindowReady(object sender, object routedEventArgs)
    {
        ApplicationHistory? latestVersion = _applicationHistoryService.GetLatestHistory();
        if(latestVersion is null)
            return;

        await SetSolution(latestVersion.Solution.FullPath);
    }

    private void InvokePropertyChangedEventArgs(params string[] propertyNames)
        => propertyNames
            .ToList()
            .ForEach(propertyName => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));

    public void PublishNugetPackage(NugetPackage? selectedItem) 
        => _nugetPublishService.Publish(selectedItem, _solutionService.CurrentSolution);

    #endregion
}

public interface IMainWindowViewModel
{
    Task BrowseForSolution();
    Task ProjectSelectionChanged(Project? selectedProject);
    Task MoveNugetPackageToProject(NugetPackage? selectedItem);
    Task MoveProjectToNugetPackage(Project? referencedProject);
    Task OnWindowReady(object sender, object routedEventArgs);
    void PublishNugetPackage(NugetPackage? selectedItem);
}