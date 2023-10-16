using CsSolutionManager.DataLayer.Entities;
using DotNet.Cli.CommandLineInterfaces;

namespace CsSolutionManager.DataLayer;

public class MapperService : IMapperService
{
    private readonly List<Mapping> _mappings = new()
    {
        new Mapping(typeof(DotNet.Cli.VisualStudio.Solution), o => 
        {
            var input = (DotNet.Cli.VisualStudio.Solution)o;

            return new Solution
            {
               Id = input.Id,
               Name = input.Name,
               Directory = input.Directory,
               FullPath = input.FullPath,
            };
        }), 
        new Mapping(typeof(DotNet.Cli.VisualStudio.NugetPackage), o =>
        {
            var input = (DotNet.Cli.VisualStudio.NugetPackage)o;

            return new NugetPackage
            {
               Id = Guid.NewGuid(),
               Name = input.Name,
               Project = input.RegisteredProject is not null 
                   ? new MapperService().Map<Project>(input.RegisteredProject)
                   : default,
               Version = input.Version,
            };
        }),
        new Mapping(typeof(DotNet.Cli.VisualStudio.Project), o =>
        {
            var input = (DotNet.Cli.VisualStudio.Project)o;

            return new Project
            {
                Id = input.Id,
                Name = input.Name,
                Directory = input.Directory
            };
        }),
        new Mapping(typeof(Project), o =>
        {
            var input = (Project)o;
            return new DotNet.Cli.VisualStudio.Project(input.Name, input.Directory);
        })
    };

    public T Map<T>(object input)
    {
        Mapping mapping = _mappings
            .SingleOrDefault(x => x.InputType == input.GetType()) 
            ?? throw new ArgumentException("Not type mappings matched the input");

        return (T)mapping.MapAction(input);
    }
}

public interface IMapperService
{
    T Map<T>(object input);
}

public class Mapping
{
    public Mapping(Type? inputType, Func<object, object> mapAction)
    {
        MapAction = mapAction;
        InputType = inputType;
    }

    public Type? InputType { get; set; }
    public Func<object, object> MapAction { get; set; }
}