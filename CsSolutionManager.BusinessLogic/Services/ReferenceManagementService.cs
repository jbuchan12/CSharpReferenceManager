using CsSolutionManager.BusinessLogic.Interfaces;
using CsSolutionManager.DataLayer;
using CsSolutionManager.DataLayer.Repositories;
using DotNet.Cli.CommandLineInterfaces;
using DotNet.Cli.VisualStudio;
using NugetPackage = DotNet.Cli.VisualStudio.NugetPackage;
using Project = DotNet.Cli.VisualStudio.Project;

namespace CsSolutionManager.BusinessLogic.Services;

public class ReferenceManagementService : IReferenceManagementService
{
    private static string CodeRepositoryFolder => $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";

    private readonly IOpenFileDialog _openFileDialog;
    private readonly IMessageBox _messageBox;
    private readonly INugetPackageRepository _nugetPackageRepository;
    private readonly IMapperService _mapper;

    public ReferenceManagementService(IOpenFileDialog openFileDialog, IMessageBox messageBox, INugetPackageRepository nugetPackageRepository, IMapperService mapper)
    {
        _openFileDialog = openFileDialog;
        _messageBox = messageBox;
        _nugetPackageRepository = nugetPackageRepository;
        _mapper = mapper;
    }

    public EventHandler<EventArgs>? SolutionFileChanged { get; set; }

    public async Task ChangeToProjectReference(NugetPackage? nugetPackage, Project? project, ISolution? solution)
    {
        if (nugetPackage is null || project is null || solution is null)
            return;

        nugetPackage.RegisteredProject = CheckForRegisteredProjectWithDb(nugetPackage); 

        if (nugetPackage.RegisteredProject is null)
            RegisterProjectFileWithNugetPackage(nugetPackage, solution);

        if (nugetPackage.RegisteredProject is null)
            return;

        await project.RemovePackage(nugetPackage);
        await project.AddProject(nugetPackage.RegisteredProject, solution);

        SolutionFileChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task ChangeToNugetReference(Project? project, NugetPackage? nugetPackage, ISolution? solution)
    {
        if (nugetPackage is null || project is null || solution is null)
            return;

        throw new NotImplementedException();
    }

    private Project? CheckForRegisteredProjectWithDb(NugetPackage nugetPackage)
    {
        DataLayer.Entities.Project? project = _nugetPackageRepository.GetByName(nugetPackage.Name)?.Project;

        return project is null 
            ? null : 
            _mapper.Map<Project>(project);
    }

    private void RegisterProjectFileWithNugetPackage(NugetPackage package, ISolution solution)
    {
        _messageBox.Show("Please register a csproj file with this nuget package before continuing");

        if (!_openFileDialog.ShowDialog(
                OpenFileDialogFilter.CsprojFileFilter,
                CodeRepositoryFolder,
                "Please select a csproj file.") ?? false)
            return;

        var project = new Project(
            _openFileDialog.FileName,
            new ProjectsCommandLineInterface<ISolution>(solution,string.Empty));

        package.RegisteredProject = project;

        _ = _nugetPackageRepository.Add(
            _mapper.Map<DataLayer.Entities.NugetPackage>(package));
    }
}

public interface IReferenceManagementService
{
    Task ChangeToProjectReference(NugetPackage? nugetPackage, Project? selectedProject, ISolution? solution);
    Task ChangeToNugetReference(Project? project, NugetPackage? nugetPackage, ISolution? solution);
}