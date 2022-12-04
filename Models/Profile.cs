using System.ComponentModel.DataAnnotations;

namespace QualificationSite.Models;

public class Profile
{
    public long Id { get; set; }
    [MaxLength(50)]
    public string? Name { get; set; }
    [MaxLength(50)]
    public string? Surname { get; set; }
    public int? Age { get; set; }

    [MaxLength(100)]
    public string? Languages { get; set; }

    [MaxLength(100)]
    public string? University { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }
}