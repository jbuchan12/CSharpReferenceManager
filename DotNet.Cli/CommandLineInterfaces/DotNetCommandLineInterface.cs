using DotNet.Cli.VisualStudio;

namespace DotNet.Cli.CommandLineInterfaces
{
    public class DotNetCommandLineInterface : CommandLineInterface
    {
        public SolutionCommandLineInterface Solution(ISolution solution)
            => new(solution, Command);
    }
}
