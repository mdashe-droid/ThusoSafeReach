using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThusoSafeReach.Data;
using ThusoSafeReach.Models;

namespace ThusoSafeReach.Controllers;

[Authorize]
public class SOSController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public SOSController(
        ApplicationDbContext db,
        UserManager<IdentityUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User);

        if (User.IsInRole("Coordinator") || User.IsInRole("Admin"))
        {
            return View(await _db.SOSAlerts
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync());
        }

        return View(await _db.SOSAlerts
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string? message, double? latitude, double? longitude)
    {
        if (latitude.HasValue &&
            (latitude < -90 || latitude > 90))
            ModelState.AddModelError("latitude", "Invalid latitude.");

        if (longitude.HasValue &&
            (longitude < -180 || longitude > 180))
            ModelState.AddModelError("longitude", "Invalid longitude.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var alert = new SOSAlert
        {
            UserId = _users.GetUserId(User)!,
            Message = string.IsNullOrWhiteSpace(message)
                ? "Emergency assistance requested"
                : message.Trim()[..Math.Min(message.Trim().Length, 500)],
            Latitude = latitude,
            Longitude = longitude,
            CreatedAtUtc = DateTime.UtcNow,
            Status = "Active"
        };

        _db.SOSAlerts.Add(alert);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Coordinator,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resolve(int id)
    {
        var alert = await _db.SOSAlerts.FindAsync(id);

        if (alert == null)
            return NotFound();

        if (alert.Status != "Resolved")
        {
            alert.Status = "Resolved";
            alert.HandledByUserId = _users.GetUserId(User);
            alert.ResolvedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}