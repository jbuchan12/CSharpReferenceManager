namespace DotNet.Cli.VisualStudio;

public class ProjectChangeLog
{
    public ProjectChangeLog(ProjectChangedObject changedObject, int changeCount)
    {
        ChangedObject = changedObject;
        ChangeCount = changeCount;
    }

    public ProjectChangedObject ChangedObject { get; set; }
    public int ChangeCount { get; set; }
}

public enum ProjectChangedObject
{
    None,
    Project,
    Package
}