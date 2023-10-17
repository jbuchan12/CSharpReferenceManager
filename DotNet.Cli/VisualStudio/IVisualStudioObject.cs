namespace DotNet.Cli.VisualStudio;

public interface IVisualStudioObject
{
    Guid Id { get; set; }
    string Name { get; }
    string Directory { get; }
    Task RemoveProject(Project project, ISolution solution);
}