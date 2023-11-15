using System;
using CsSolutionManager.BusinessLogic.Services;

namespace CsSolutionManager.UI;

public class ApplicationService : IApplicationService
{
    public Action<string>? OutputLabelWriteAction { get; set; }
    public void WriteToOutputLabel(string message) 
        => OutputLabelWriteAction?.Invoke(message);
}