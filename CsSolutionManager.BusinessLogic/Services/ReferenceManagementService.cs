using CsSolutionManager.BusinessLogic.Interfaces;
using DotNet.Cli.CommandLineInterfaces;
using DotNet.Cli.VisualStudio;

namespace CsSolutionManager.BusinessLogic.Services;

public interface IMessageBox
{
    void Show(string message);
}

public class ReferenceManagementService : IReferenceManagementService
{
    private static string CodeRepositoryFolder => $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";
    private readonly IOpenFileDialog _openFileDialog;
    private readonly IMessageBox _messageBox;
    private readonly DotNetCommandLineInterface _dotNetCli;

    public ReferenceManagementService(IOpenFileDialog openFileDialog, IMessageBox messageBox)
    {
        _openFileDialog = openFileDialog;
        _messageBox = messageBox;
        _dotNetCli = new DotNetCommandLineInterface();
    }

    public async Task ChangeToProjectReference(NugetPackage? nugetPackage, Project? project, ISolution? solution)
    {
        if (nugetPackage is null || project is null || solution is null)
            return;

        if (nugetPackage.RegisteredProject is null)
            RegisterProjectFileWithNugetPackage(nugetPackage, solution);

        if (nugetPackage.RegisteredProject is null)
            return;

        await project.RemovePackage(nugetPackage);
        await project.AddProject(nugetPackage.RegisteredProject, solution);
    }

    public Task ChangeToNugetReference(Project? project, NugetPackage? nugetPackage, ISolution? solution)
    {
        throw new NotImplementedException();
    }

    private void RegisterProjectFileWithNugetPackage(NugetPackage package, ISolution solution)
    {
        _messageBox.Show("Please register a csproj file with this nuget package before continuing");

        if (!_openFileDialog.ShowDialog(
                OpenFileDialogFilter.CsprojFileFilter,
                CodeRepositoryFolder,
                "Please select a csproj file.") ?? false)
            return;

        var project = new Project(_openFileDialog.FileName, _dotNetCli.Solution(solution).Projects);
        package.RegisterWithProject(project);
    }
}

public interface IReferenceManagementService
{
    Task ChangeToProjectReference(NugetPackage? nugetPackage, Project? selectedProject, ISolution? solution);
    Task ChangeToNugetReference(Project? project, NugetPackage? nugetPackage, ISolution? solution);
}