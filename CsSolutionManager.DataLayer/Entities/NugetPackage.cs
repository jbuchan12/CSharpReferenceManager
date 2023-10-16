using System.ComponentModel.DataAnnotations;

namespace CsSolutionManager.DataLayer.Entities;

public class NugetPackage : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public Project? Project { get; set; }
}