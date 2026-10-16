using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThusoSafeReach.Data;
using ThusoSafeReach.Models;

namespace ThusoSafeReach.Controllers;

[Authorize]
public class MedicalController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public MedicalController(
        ApplicationDbContext db,
        UserManager<IdentityUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User);

        var profile = await _db.MedicalProfiles
            .FirstOrDefaultAsync(m => m.UserId == userId);

        return View(profile ?? new MedicalProfile());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(MedicalProfile input)
    {
        ModelState.Remove(nameof(input.UserId));

        if (!ModelState.IsValid)
            return View("Index", input);

        var userId = _users.GetUserId(User)!;

        var profile = await _db.MedicalProfiles
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (profile == null)
        {
            profile = new MedicalProfile { UserId = userId };
            _db.MedicalProfiles.Add(profile);
        }

        profile.Allergies = input.Allergies;
        profile.Medication = input.Medication;
        profile.MedicalNotes = input.MedicalNotes;
        profile.EmergencyBloodType = input.EmergencyBloodType;

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}