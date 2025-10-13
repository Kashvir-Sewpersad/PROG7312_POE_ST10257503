using Microsoft.AspNetCore.Mvc;
using Programming_7312_Part_1.Models;
using Programming_7312_Part_1.Services;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Programming_7312_Part_1.Controllers
{
    public class AdminController : Controller
    {
        private readonly IssueStorage _issueStorage;
        private readonly EventService _eventService;
        private readonly AnnouncementService _announcementService;

        public AdminController(IssueStorage issueStorage, EventService eventService, AnnouncementService announcementService)
        {
            _issueStorage = issueStorage;
            _eventService = eventService;
            _announcementService = announcementService;
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
            ViewBag.Announcements = _announcementService.GetAllAnnouncements();
            return View();
        }

        // GET: Admin/EditEvent
        public IActionResult EditEvent(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var eventItem = _eventService.GetEventById(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            ViewBag.TagsInput = string.Join(", ", eventItem.Tags ?? new List<string>());
            ViewBag.Categories = _eventService.UniqueCategories.ToList();
            ViewBag.Tags = _eventService.UniqueTags.ToList();
            return View(eventItem);
        }

        // GET: Admin/CreateEvent
        public IActionResult CreateEvent()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Categories = _eventService.UniqueCategories.ToList();
            ViewBag.Tags = _eventService.UniqueTags.ToList();
            return View();
        }

        // POST: Admin/CreateEvent
        [HttpPost]
        public async Task<IActionResult> CreateEvent(Event model, string tagsInput, IFormFile imageFile)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Validate category
                var validCategories = _eventService.UniqueCategories.ToList();
                if (!validCategories.Contains(model.Category))
                {
                    ModelState.AddModelError("Category", "Please select a valid category from the dropdown.");
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    return View(model);
                }

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    model.ImagePath = "/uploads/" + uniqueFileName;
                }

                // Parse tags
                if (!string.IsNullOrWhiteSpace(tagsInput))
                {
                    model.Tags = tagsInput.Split(',')
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();
                }
                else
                {
                    model.Tags = new List<string>();
                }

                // Generate new ID
                var allEvents = _eventService.GetAllEvents();
                model.Id = allEvents.Any() ? allEvents.Max(e => e.Id) + 1 : 1;

                _eventService.AddEvent(model);

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the event. Please try again.");
                ViewBag.Categories = _eventService.UniqueCategories.ToList();
                ViewBag.Tags = _eventService.UniqueTags.ToList();
                return View(model);
            }
        }

        // POST: Admin/EditEvent
        [HttpPost]
        public IActionResult EditEvent(Event model, string tagsInput)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Validate category
                var validCategories = _eventService.UniqueCategories.ToList();
                if (!validCategories.Contains(model.Category))
                {
                    ModelState.AddModelError("Category", "Please select a valid category from the dropdown.");
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    ViewBag.TagsInput = string.Join(", ", model.Tags ?? new List<string>());
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    ViewBag.TagsInput = string.Join(", ", model.Tags ?? new List<string>());
                    return View(model);
                }

                // Parse tags
                if (!string.IsNullOrWhiteSpace(tagsInput))
                {
                    model.Tags = tagsInput.Split(',')
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();
                }
                else
                {
                    model.Tags = new List<string>();
                }

                var success = _eventService.UpdateEvent(model);
                if (!success)
                {
                    ModelState.AddModelError("", "Failed to update event.");
                    ViewBag.Categories = validCategories;
                    ViewBag.Tags = _eventService.UniqueTags.ToList();
                    ViewBag.TagsInput = string.Join(", ", model.Tags ?? new List<string>());
                    return View(model);
                }

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the event. Please try again.");
                ViewBag.Categories = _eventService.UniqueCategories.ToList();
                ViewBag.Tags = _eventService.UniqueTags.ToList();
                ViewBag.TagsInput = string.Join(", ", model.Tags ?? new List<string>());
                return View(model);
            }
        }

        // POST: Admin/DeleteEvent
        [HttpPost]
        public IActionResult DeleteEvent(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var success = _eventService.DeleteEvent(id);
            if (!success)
            {
                TempData["Error"] = "Failed to delete event.";
            }
            else
            {
                TempData["Success"] = "Event deleted successfully.";
            }

            return RedirectToAction("Dashboard");
        }

        // GET: Admin/CreateAnnouncement
        public IActionResult CreateAnnouncement()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Categories = _announcementService.UniqueCategories.ToList();
            return View();
        }

        // POST: Admin/CreateAnnouncement
        [HttpPost]
        public IActionResult CreateAnnouncement(Announcement model)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = _announcementService.UniqueCategories.ToList();
                    return View(model);
                }

                // Generate new ID
                var allAnnouncements = _announcementService.GetAllAnnouncements();
                model.Id = allAnnouncements.Any() ? allAnnouncements.Max(a => a.Id) + 1 : 1;

                _announcementService.AddAnnouncement(model);

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the announcement. Please try again.");
                ViewBag.Categories = _announcementService.UniqueCategories.ToList();
                return View(model);
            }
        }

        // POST: Admin/DeleteAnnouncement
        [HttpPost]
        public IActionResult DeleteAnnouncement(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var success = _announcementService.DeleteAnnouncement(id);
            if (!success)
            {
                TempData["Error"] = "Failed to delete announcement.";
            }
            else
            {
                TempData["Success"] = "Announcement deleted successfully.";
            }

            return RedirectToAction("Dashboard");
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