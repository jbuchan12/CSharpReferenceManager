namespace CsSolutionManager.BusinessLogic.Interfaces;

public interface IOpenFileDialog
{
    string Filter { get; set; }
    string InitialDirectory { get; set; }
    string Title { get; set; }
    string FileName { get; set; }
    bool? ShowDialog();
    bool? ShowDialog(string filter, string initialDirectory, string title);
}

public static class OpenFileDialogFilter
{
    public const string CsprojFileFilter = "CsProj Files (*.csproj)|*.csproj";
    public const string SolutionFileFilter = "Solution Files (*.sln)|*.sln";
}