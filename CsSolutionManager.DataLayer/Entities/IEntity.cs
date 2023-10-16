using System.ComponentModel.DataAnnotations;

namespace CsSolutionManager.DataLayer.Entities;

public interface IEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
}