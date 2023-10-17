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
    private readonly IProjectRepository _projectRepository;
    private readonly IMapperService _mapper;

    public ReferenceManagementService(
        IOpenFileDialog openFileDialog, 
        IMessageBox messageBox, 
        INugetPackageRepository nugetPackageRepository, 
        IProjectRepository projectRepository, 
        IMapperService mapper)
    {
        _openFileDialog = openFileDialog;
        _messageBox = messageBox;
        _nugetPackageRepository = nugetPackageRepository;
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

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
        await project.RefreshData();
    }

    public async Task ChangeToNugetReference(Project? selectedProject, Project? referencedProject, ISolution? solution)
    {
        if (selectedProject is null || referencedProject is null || solution is null)
            return;

        DataLayer.Entities.Project? dbProject = _projectRepository.GetIncludingNugetPackage(referencedProject.Name);
        if (dbProject?.NugetPackage is not null)
        {
            referencedProject.LinkedNugetPackage = _mapper.MapTo<NugetPackage>(dbProject.NugetPackage);
        }

        if (referencedProject.LinkedNugetPackage is null)
            return;

        await selectedProject.RemoveProject(referencedProject, solution);
        await selectedProject.AddPackage(referencedProject.LinkedNugetPackage);
        await selectedProject.RefreshData();
    }

    private Project? CheckForRegisteredProjectWithDb(NugetPackage nugetPackage)
    {
        DataLayer.Entities.Project? project = _nugetPackageRepository.GetByName(nugetPackage.Name)?.Project;

        return project is null 
            ? null : 
            _mapper.MapTo<Project>(project);
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
            _mapper.MapTo<DataLayer.Entities.NugetPackage>(package));
    }
}

public interface IReferenceManagementService
{
    Task ChangeToProjectReference(NugetPackage? nugetPackage, Project? selectedProject, ISolution? solution);
    Task ChangeToNugetReference(Project? selectedProject, Project? referencedProject, ISolution? solution);
}