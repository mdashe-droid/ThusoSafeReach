using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThusoSafeReach.Data;
using ThusoSafeReach.Models;

namespace ThusoSafeReach.Controllers;

[Authorize]
public class CheckInsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public CheckInsController(
        ApplicationDbContext db,
        UserManager<IdentityUser> users)
    {
        _db = db;
        _users = users;
    }
    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User);

        var items = await _db.CheckIns
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync();

        return View(items);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string destination, int durationMinutes)
    {
        if (string.IsNullOrWhiteSpace(destination) ||
            destination.Length > 150)
        {
            ModelState.AddModelError(
                nameof(destination), "Enter a valid destination.");
        }

        if (durationMinutes < 1 || durationMinutes > 1440)
        {
            ModelState.AddModelError(
                nameof(durationMinutes),
                "Choose a duration between 1 minute and 24 hours.");
        }

        if (!ModelState.IsValid)
            return View();

        var checkIn = new CheckIn
        {
            UserId = _users.GetUserId(User)!,
            Destination = destination.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            DueAtUtc = DateTime.UtcNow.AddMinutes(durationMinutes),
            Status = "Active"
        };

        _db.CheckIns.Add(checkIn);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var userId = _users.GetUserId(User);

        var checkIn = await _db.CheckIns.FirstOrDefaultAsync(
            c => c.Id == id && c.UserId == userId);

        if (checkIn == null)
            return NotFound();

        if (checkIn.Status == "Active")
        {
            checkIn.Status = "Completed";
            checkIn.CompletedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}