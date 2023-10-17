using CsSolutionManager.DataLayer.Entities;

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
                   ? new MapperService().MapTo<Project>(input.RegisteredProject)
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
        }),
        new Mapping(typeof(NugetPackage), o =>
        {
            var input = (NugetPackage)o;
            return new DotNet.Cli.VisualStudio.NugetPackage(input.Name, input.Version)
            {
                RegisteredProject = input.Project is not null
                    ? new MapperService().MapTo<DotNet.Cli.VisualStudio.Project>(input.Project)
                    : default,
            };
        })
    };

    public T MapTo<T>(object inputObject)
    {
        Mapping mapping = _mappings
            .SingleOrDefault(x => x.InputType == inputObject.GetType()) 
            ?? throw new ArgumentException("Not type mappings matched the input");

        return (T)mapping.MapAction(inputObject);
    }
}

public interface IMapperService
{
    T MapTo<T>(object inputObject);
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