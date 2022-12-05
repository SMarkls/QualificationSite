using System.ComponentModel.DataAnnotations;

namespace QualificationSite.ViewModel;

public class ProfilePinViewModel
{
    public long ProfileId { get; set; }
    [MaxLength(50)]
    public string? Header { get; set; }
    [MaxLength(250)]
    public string? Text { get; set; }
    [MaxLength(100)]
    public string? LinkToAttachment { get; set; }
}