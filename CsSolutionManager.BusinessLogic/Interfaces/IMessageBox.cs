namespace CsSolutionManager.BusinessLogic.Interfaces;

public interface IMessageBox
{
    void Show(string message);
    bool AskQuestion(string question, string caption);
}