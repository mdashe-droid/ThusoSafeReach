using System.ComponentModel.DataAnnotations;

namespace ThusoSafeReach.Models;

public class SOSAlert
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    [Required, StringLength(500)]
    public string Message { get; set; } = "Emergency assistance requested";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    [StringLength(30)]
    public string Status { get; set; } = "Active";
    public string? HandledByUserId { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
}