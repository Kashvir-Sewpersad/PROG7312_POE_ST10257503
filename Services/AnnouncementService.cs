// Services/AnnouncementService.cs
using Programming_7312_Part_1.Data;
using Programming_7312_Part_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Programming_7312_Part_1.Services
{
    public class AnnouncementService
    {
        private readonly ApplicationDbContext _context;

        // Queue for managing announcements (FIFO - oldest first)
        public Queue<Announcement> AnnouncementQueue { get; } = new Queue<Announcement>();

        // Dictionary for storing announcements by category
        public Dictionary<string, List<Announcement>> AnnouncementsByCategory { get; } = new Dictionary<string, List<Announcement>>();

        // HashSet for unique announcement categories
        public HashSet<string> UniqueCategories { get; } = new HashSet<string>();

        // SortedDictionary for announcements by priority (higher priority first)
        public SortedDictionary<int, List<Announcement>> AnnouncementsByPriority { get; } = new SortedDictionary<int, List<Announcement>>();

        public AnnouncementService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            InitializeDataStructures();
        }

        private void InitializeDataStructures()
        {
            // Load all announcements from database
            var allAnnouncements = _context.Announcements.ToList();

            foreach (var announcement in allAnnouncements)
            {
                AddAnnouncementToDataStructures(announcement);
            }

            // If no announcements exist, seed sample announcements
            if (!allAnnouncements.Any())
            {
                SeedSampleAnnouncements();
            }
        }

        private void SeedSampleAnnouncements()
        {
            var sampleAnnouncements = new List<Announcement>
            {
                new Announcement
                {
                    Title = "Water Maintenance Notice",
                    Content = "There will be no water in the area of Upper Newlands on 18 November 2025 from 08:00 to 17:00 due to planned maintenance of water infrastructure - Municipality.",
                    Category = "Maintenance",
                    Priority = 3,
                    ExpiryDate = DateTime.Now.AddDays(30)
                },
                new Announcement
                {
                    Title = "Community Meeting",
                    Content = "Join us for the monthly community meeting to discuss local issues and improvements.",
                    Category = "Community",
                    Priority = 2,
                    ExpiryDate = DateTime.Now.AddDays(7)
                }
            };

            foreach (var announcement in sampleAnnouncements)
            {
                _context.Announcements.Add(announcement);
            }
            _context.SaveChanges();

            // Now add to data structures
            foreach (var announcement in sampleAnnouncements)
            {
                AddAnnouncementToDataStructures(announcement);
            }
        }

        public void AddAnnouncement(Announcement announcement)
        {
            _context.Announcements.Add(announcement);
            _context.SaveChanges();

            AddAnnouncementToDataStructures(announcement);
        }

        private void AddAnnouncementToDataStructures(Announcement announcement)
        {
            // Add to AnnouncementQueue (keep only last 10)
            AnnouncementQueue.Enqueue(announcement);
            if (AnnouncementQueue.Count > 10)
            {
                AnnouncementQueue.Dequeue();
            }

            // Add to AnnouncementsByCategory
            if (!string.IsNullOrEmpty(announcement.Category))
            {
                if (!AnnouncementsByCategory.ContainsKey(announcement.Category))
                {
                    AnnouncementsByCategory[announcement.Category] = new List<Announcement>();
                }
                AnnouncementsByCategory[announcement.Category].Add(announcement);

                // Add to UniqueCategories
                UniqueCategories.Add(announcement.Category);
            }

            // Add to AnnouncementsByPriority
            if (!AnnouncementsByPriority.ContainsKey(announcement.Priority))
            {
                AnnouncementsByPriority[announcement.Priority] = new List<Announcement>();
            }
            AnnouncementsByPriority[announcement.Priority].Add(announcement);
        }

        public List<Announcement> GetAllAnnouncements()
        {
            return _context.Announcements
                .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now))
                .OrderByDescending(a => a.Priority)
                .ThenByDescending(a => a.CreatedDate)
                .ToList();
        }

        public List<Announcement> GetActiveAnnouncements(int count = 5)
        {
            return GetAllAnnouncements().Take(count).ToList();
        }

        public List<Announcement> GetAnnouncementsByCategory(string category)
        {
            if (AnnouncementsByCategory.ContainsKey(category))
            {
                return AnnouncementsByCategory[category]
                    .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now))
                    .OrderByDescending(a => a.CreatedDate)
                    .ToList();
            }
            return new List<Announcement>();
        }

        public List<Announcement> GetRecentAnnouncements(int count = 3)
        {
            return AnnouncementQueue.Reverse().Take(count).ToList();
        }

        public Announcement GetAnnouncementById(int id)
        {
            return _context.Announcements.FirstOrDefault(a => a.Id == id);
        }

        public bool UpdateAnnouncement(Announcement updatedAnnouncement)
        {
            var existingAnnouncement = _context.Announcements.FirstOrDefault(a => a.Id == updatedAnnouncement.Id);
            if (existingAnnouncement == null)
            {
                return false;
            }

            // Store old values for cleanup
            var oldCategory = existingAnnouncement.Category;
            var oldPriority = existingAnnouncement.Priority; // Store the old priority

            // Update properties
            existingAnnouncement.Title = updatedAnnouncement.Title;
            existingAnnouncement.Content = updatedAnnouncement.Content;
            existingAnnouncement.Category = updatedAnnouncement.Category;
            existingAnnouncement.ExpiryDate = updatedAnnouncement.ExpiryDate;
            existingAnnouncement.IsActive = updatedAnnouncement.IsActive;
            existingAnnouncement.Priority = updatedAnnouncement.Priority;

            _context.SaveChanges();

            // Update data structures
            UpdateAnnouncementInDataStructures(existingAnnouncement, oldCategory, oldPriority); // Pass old priority

            return true;
        }

        private void UpdateAnnouncementInDataStructures(Announcement updatedAnnouncement, string oldCategory, int oldPriority)
        {
            // If category changed, update AnnouncementsByCategory
            if (oldCategory != updatedAnnouncement.Category)
            {
                // Remove from old category
                if (!string.IsNullOrEmpty(oldCategory) && AnnouncementsByCategory.ContainsKey(oldCategory))
                {
                    AnnouncementsByCategory[oldCategory].RemoveAll(a => a.Id == updatedAnnouncement.Id);
                    if (AnnouncementsByCategory[oldCategory].Count == 0)
                    {
                        AnnouncementsByCategory.Remove(oldCategory);
                        UniqueCategories.Remove(oldCategory);
                    }
                }

                // Add to new category
                if (!string.IsNullOrEmpty(updatedAnnouncement.Category))
                {
                    if (!AnnouncementsByCategory.ContainsKey(updatedAnnouncement.Category))
                    {
                        AnnouncementsByCategory[updatedAnnouncement.Category] = new List<Announcement>();
                    }
                    AnnouncementsByCategory[updatedAnnouncement.Category].Add(updatedAnnouncement);

                    // Update UniqueCategories
                    UniqueCategories.Add(updatedAnnouncement.Category);
                }
            }

            // If priority changed, update AnnouncementsByPriority
            if (oldPriority != updatedAnnouncement.Priority)
            {
                // Remove from old priority
                if (AnnouncementsByPriority.ContainsKey(oldPriority))
                {
                    AnnouncementsByPriority[oldPriority].RemoveAll(a => a.Id == updatedAnnouncement.Id);
                    if (AnnouncementsByPriority[oldPriority].Count == 0)
                    {
                        AnnouncementsByPriority.Remove(oldPriority);
                    }
                }
            }

            // Add to new priority (or update in existing)
            if (!AnnouncementsByPriority.ContainsKey(updatedAnnouncement.Priority))
            {
                AnnouncementsByPriority[updatedAnnouncement.Priority] = new List<Announcement>();
            }
            // Ensure the announcement is not duplicated if priority hasn't changed
            AnnouncementsByPriority[updatedAnnouncement.Priority].RemoveAll(a => a.Id == updatedAnnouncement.Id);
            AnnouncementsByPriority[updatedAnnouncement.Priority].Add(updatedAnnouncement);
        }

        public bool DeleteAnnouncement(int id)
        {
            var announcement = _context.Announcements.FirstOrDefault(a => a.Id == id);
            if (announcement == null)
            {
                return false;
            }

            // Store old values for cleanup
            var oldCategory = announcement.Category;

            // Remove from database
            _context.Announcements.Remove(announcement);
            _context.SaveChanges();

            // Update data structures
            RemoveAnnouncementFromDataStructures(announcement, oldCategory);

            return true;
        }

        private void RemoveAnnouncementFromDataStructures(Announcement announcement, string oldCategory)
        {
            // Remove from AnnouncementsByCategory
            if (!string.IsNullOrEmpty(oldCategory) && AnnouncementsByCategory.ContainsKey(oldCategory))
            {
                AnnouncementsByCategory[oldCategory].RemoveAll(a => a.Id == announcement.Id);
                if (AnnouncementsByCategory[oldCategory].Count == 0)
                {
                    AnnouncementsByCategory.Remove(oldCategory);
                    UniqueCategories.Remove(oldCategory);
                }
            }

            // Remove from AnnouncementQueue
            var queueList = AnnouncementQueue.Where(a => a.Id != announcement.Id).ToList();
            AnnouncementQueue.Clear();
            foreach (var a in queueList)
            {
                AnnouncementQueue.Enqueue(a);
            }

            // Remove from AnnouncementsByPriority
            foreach (var priorityList in AnnouncementsByPriority.Values)
            {
                priorityList.RemoveAll(a => a.Id == announcement.Id);
            }
        }
    }
}