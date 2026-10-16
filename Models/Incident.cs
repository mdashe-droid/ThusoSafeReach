using System.ComponentModel.DataAnnotations;

namespace ThusoSafeReach.Models;

public class Incident
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    [Required, StringLength(120)]
    public string Title { get; set; } = "";
    [Required, StringLength(2000)]
    public string Description { get; set; } = "";
    [StringLength(40)]
    public string Category { get; set; } = "Other";
    [StringLength(30)]
    public string Status { get; set; } = "Open";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}