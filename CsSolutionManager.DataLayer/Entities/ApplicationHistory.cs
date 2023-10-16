using System.ComponentModel.DataAnnotations;

namespace CsSolutionManager.DataLayer.Entities;

public class ApplicationHistory : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Solution Solution { get; set; } = new ();
    public DateTime Date { get; set; }
}