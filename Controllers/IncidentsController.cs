using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThusoSafeReach.Data;
using ThusoSafeReach.Models;

namespace ThusoSafeReach.Controllers;

[Authorize]
public class IncidentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public IncidentsController(
        ApplicationDbContext db,
        UserManager<IdentityUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User);
        var query = _db.Incidents.AsQueryable();

        if (!User.IsInRole("Coordinator") &&
            !User.IsInRole("Admin"))
        {
            query = query.Where(i => i.UserId == userId);
        }

        return View(await query
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync());
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Incident incident)
    {
        ModelState.Remove(nameof(incident.UserId));
        ModelState.Remove(nameof(incident.CreatedAtUtc));
        ModelState.Remove(nameof(incident.Status));

        if (!ModelState.IsValid)
            return View(incident);

        incident.Id = 0;
        incident.UserId = _users.GetUserId(User)!;
        incident.CreatedAtUtc = DateTime.UtcNow;
        incident.Status = "Open";

        _db.Incidents.Add(incident);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = "Coordinator,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
        int id, string status)
    {
        var allowed = new[] { "Open", "UnderReview", "Resolved" };

        if (!allowed.Contains(status))
            return BadRequest();

        var incident = await _db.Incidents.FindAsync(id);

        if (incident == null)
            return NotFound();

        incident.Status = status;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}