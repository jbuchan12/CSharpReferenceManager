namespace CsSolutionManger.Console.DotNetCli
{
    public class DotNetCli : CliApi
    {
        public SolutionCliApi Solution(ISolution solution) 
            => new(solution, Command);
    }
}
