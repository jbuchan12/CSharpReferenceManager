namespace CsSolutionManger.Console;

public class Console : IConsole
{
    public void WriteLine(string message) => System.Console.WriteLine(message);
    public ConsoleKeyInfo ReadKey() => System.Console.ReadKey();
}

public interface IConsole
{
    void WriteLine(string message);
    ConsoleKeyInfo ReadKey();
}