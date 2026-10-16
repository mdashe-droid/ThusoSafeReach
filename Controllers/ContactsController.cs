using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThusoSafeReach.Data;
using ThusoSafeReach.Models;

namespace ThusoSafeReach.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public ContactsController(
        ApplicationDbContext db,
        UserManager<IdentityUser> users)
    {
        _db = db;
        _users = users;
    }
    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User);

        return View(await _db.EmergencyContacts
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync());
    }
    public IActionResult Create() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmergencyContact contact)
    {
        ModelState.Remove(nameof(contact.UserId));

        if (!ModelState.IsValid)
            return View(contact);

        contact.Id = 0;
        contact.UserId = _users.GetUserId(User)!;

        _db.EmergencyContacts.Add(contact);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _users.GetUserId(User);

        var contact = await _db.EmergencyContacts
            .FirstOrDefaultAsync(c =>
                c.Id == id && c.UserId == userId);

        if (contact == null)
            return NotFound();

        _db.EmergencyContacts.Remove(contact);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}