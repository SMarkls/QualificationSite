using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualificationSite.Models;

public class ProfilePin
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("profile")]
    public long ProfileId { get; set; }
    [MaxLength(50)]
    public string? Header { get; set; }
    [MaxLength(250)]
    public string? Text { get; set; }
    [MaxLength(100)]
    public string? LinkToAttachment { get; set; }
}