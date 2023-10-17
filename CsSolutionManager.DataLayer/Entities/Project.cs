using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CsSolutionManager.DataLayer.Entities;

public class Project : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
    [ForeignKey("NugetPackage")]
    public Guid NugetPackageId { get; set; }
    [NotMapped]
    public NugetPackage? NugetPackage { get; set; }
}