using System;
using System.Windows;
using CsSolutionManager.BusinessLogic.Interfaces;

namespace CsSolutionManager.UI.UIWrappers;

public class MessageBoxWrapper : IMessageBox
{
    public void Show(string message) => MessageBox.Show(message);

    public bool AskQuestion(string question, string caption) 
        => MessageBox.Show(question, caption, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
}