using System.ComponentModel.DataAnnotations;

namespace ThusoSafeReach.Models;

public class CheckIn
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    [Required, StringLength(150)]
    public string Destination { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime DueAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    [StringLength(30)]
    public string Status { get; set; } = "Active";
}