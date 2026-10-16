using System.ComponentModel.DataAnnotations;

namespace ThusoSafeReach.Models;

public class EmergencyContact
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    [Required, StringLength(100)]
    public string Name { get; set; } = "";
    [Required, Phone, StringLength(30)]
    public string PhoneNumber { get; set; } = "";
    [StringLength(60)]
    public string? Relationship { get; set; }
}