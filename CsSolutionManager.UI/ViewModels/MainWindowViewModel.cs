using System;
using System.Collections.Generic;
using System.ComponentModel;
using CsSolutionManger.Console;
using CsSolutionManger.Console.DotNetCli;
using CsSolutionManger.Console.Interfaces;

namespace CsSolutionManager.UI.ViewModels
{
    public class MainWindowViewModel : IMainWindowViewModel, INotifyPropertyChanged
    {
        private readonly IOpenFileDialog _openFileDialog;
        private ISolution? _solution;

        public MainWindowViewModel(IOpenFileDialog openFileDialog)
        {
            _openFileDialog = openFileDialog;
        }

        public string SolutionPath { get; set; } = string.Empty;
        public List<Project> Projects { get; set; } = new ();
        public List<NugetPackage> NugetPackages { get; set; } = new();
        public List<Project> ProjectReferences { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public void BrowseForSolution()
        {
            _openFileDialog.Filter = "Solution Files (*.sln)|*.sln";
            _openFileDialog.InitialDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\source\repos\";
            _openFileDialog.Title = "Please select a solution file.";

            if (!(_openFileDialog.ShowDialog() ?? false)) 
                return;

            InitialiseSolutionData();
        }

        public void ProjectSelectionChanged(Project? selectedProject)
        {
            if(selectedProject == null) 
                return;

            NugetPackages = selectedProject.Packages;
            ProjectReferences = selectedProject.Projects;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NugetPackages)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProjectReferences)));
        }

        private void InitialiseSolutionData()
        {
            SolutionPath = _openFileDialog.FileName;
            _solution = new Solution(SolutionPath, new DotNetCli());
            Projects = _solution.Projects;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SolutionPath)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Projects)));
        }
    }

    public interface IMainWindowViewModel
    {
        string SolutionPath { get; set; }
        void BrowseForSolution();
        void ProjectSelectionChanged(Project? selectedProject);
    }
}