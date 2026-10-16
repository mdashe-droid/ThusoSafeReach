using System.ComponentModel.DataAnnotations;

namespace ThusoSafeReach.Models;

public class MedicalProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    [StringLength(500)]
    public string? Allergies { get; set; }
    [StringLength(500)]
    public string? Medication { get; set; }
    [StringLength(1000)]
    public string? MedicalNotes { get; set; }
    [StringLength(30)]
    public string? EmergencyBloodType { get; set; }
}