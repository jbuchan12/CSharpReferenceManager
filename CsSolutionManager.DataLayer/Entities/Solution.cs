using System.ComponentModel.DataAnnotations;

namespace CsSolutionManager.DataLayer.Entities;

public class Solution : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
}