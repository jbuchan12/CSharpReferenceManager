namespace DotNet.Cli.VisualStudio;

public class NugetPackageVersion
{
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }
    public int PatchVersion { get; set; }

    public static explicit operator NugetPackageVersion(string versionString)
    {
        if (! versionString.Contains('.'))
            throw new InvalidCastException("The string was provided in the wrong format, should be 1.0.0");

        string[] versionSplit = versionString.Split('.');

        if(versionSplit.Length != 3)
            throw new InvalidCastException("The string was provided in the wrong format, should be 1.0.0");

        return new NugetPackageVersion
        {
            MajorVersion = int.Parse(versionSplit[0]),
            MinorVersion = int.Parse(versionSplit[1]),
            PatchVersion = int.Parse(versionSplit[2])
        };
    }

    public override string ToString() => $"{MajorVersion}.{MinorVersion}.{PatchVersion}";
}