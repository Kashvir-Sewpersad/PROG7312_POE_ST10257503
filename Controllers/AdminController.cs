using Microsoft.AspNetCore.Mvc;
using Programming_7312_Part_1.Services;
using System.Linq;

namespace Programming_7312_Part_1.Controllers
{
    public class AdminController : Controller
    {
        private readonly IssueStorage _issueStorage;
        private readonly EventService _eventService;

        public AdminController(IssueStorage issueStorage, EventService eventService)
        {
            _issueStorage = issueStorage;
            _eventService = eventService;
        }

        // GET: Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        public IActionResult Login(string password)
        {
            if (password == "1234")
            {
                // Simple session-based auth
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.Error = "Invalid password.";
                return View();
            }
        }

        // GET: Admin/Dashboard
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Issues = _issueStorage.ReportedIssues.ToList();
            ViewBag.Events = _eventService.GetAllEvents();
            return View();
        }

        // POST: Admin/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLoggedIn");
            return RedirectToAction("Index", "Home");
        }
    }
}