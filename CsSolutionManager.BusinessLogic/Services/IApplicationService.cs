namespace CsSolutionManager.BusinessLogic.Services;

public interface IApplicationService
{
    Action<string>? OutputLabelWriteAction { get; set; }

    public void WriteToOutputLabel(string message);
}