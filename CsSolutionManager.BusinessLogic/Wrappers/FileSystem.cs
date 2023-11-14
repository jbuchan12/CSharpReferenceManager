namespace CsSolutionManager.BusinessLogic.Wrappers;

public class FileSystem : IFileSystem
{
    public string GetUserFolder() => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public string GetUserNugetFolder() => $@"{GetUserFolder()}\.nuget\";
}

public interface IFileSystem
{
    string GetUserFolder();
}