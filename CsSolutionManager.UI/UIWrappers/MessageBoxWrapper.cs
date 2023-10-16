using System.Windows;
using CsSolutionManager.BusinessLogic.Interfaces;

namespace CsSolutionManager.UI.UIWrappers;

public class MessageBoxWrapper : IMessageBox
{
    public void Show(string message) => MessageBox.Show(message);
}