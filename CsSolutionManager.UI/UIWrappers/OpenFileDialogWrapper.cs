﻿using CsSolutionManager.BusinessLogic.Interfaces;

namespace CsSolutionManager.UI;

public class OpenFileDialogWrapper : IOpenFileDialog
{
    private readonly Microsoft.Win32.OpenFileDialog _dialog = new();

    public string Filter
    {
        get => _dialog.Filter; 
        set => _dialog.Filter = value;
    }

    public string InitialDirectory
    {
        get => _dialog.InitialDirectory;
        set => _dialog.InitialDirectory = value;
    }

    public string Title
    {
        get => _dialog.Title; 
        set => _dialog.Title = value;
    }

    public string FileName
    {
        get => _dialog.FileName;
        set => _dialog.FileName = value;
    }

    public bool? ShowDialog() => _dialog.ShowDialog();

    public bool? ShowDialog(string filter, string initialDirectory, string title)
    {
         Filter = filter;
         InitialDirectory = initialDirectory;
         Title = title;
         return ShowDialog();
    }
}