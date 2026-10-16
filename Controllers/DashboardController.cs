using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThusoSafeReach.Controllers;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
            return View("Admin");

        if (User.IsInRole("Coordinator"))
            return View("Coordinator");

        return View("Member");
    }
}