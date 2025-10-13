// Services/ContactService.cs
using Programming_7312_Part_1.Data;
using Programming_7312_Part_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Programming_7312_Part_1.Services
{
    public class ContactService
    {
        private readonly ApplicationDbContext _context;

        // Queue for managing contact messages (FIFO - oldest first)
        public Queue<Contact> ContactQueue { get; } = new Queue<Contact>();

        // Dictionary for storing contacts by category
        public Dictionary<string, List<Contact>> ContactsByCategory { get; } = new Dictionary<string, List<Contact>>();

        // HashSet for unique contact categories
        public HashSet<string> UniqueCategories { get; } = new HashSet<string>();

        // SortedDictionary for contacts by creation date (most recent first)
        public SortedDictionary<DateTime, List<Contact>> ContactsByDate { get; } = new SortedDictionary<DateTime, List<Contact>>();

        public ContactService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            InitializeDataStructures();
        }

        private void InitializeDataStructures()
        {
            // Load all contacts from database
            var allContacts = _context.Contacts.ToList();

            foreach (var contact in allContacts)
            {
                AddContactToDataStructures(contact);
            }
        }

        public void AddContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            _context.SaveChanges();

            AddContactToDataStructures(contact);
        }

        private void AddContactToDataStructures(Contact contact)
        {
            // Add to ContactQueue (keep only last 20)
            ContactQueue.Enqueue(contact);
            if (ContactQueue.Count > 20)
            {
                ContactQueue.Dequeue();
            }

            // Add to ContactsByCategory
            if (!string.IsNullOrEmpty(contact.Category))
            {
                if (!ContactsByCategory.ContainsKey(contact.Category))
                {
                    ContactsByCategory[contact.Category] = new List<Contact>();
                }
                ContactsByCategory[contact.Category].Add(contact);

                // Add to UniqueCategories
                UniqueCategories.Add(contact.Category);
            }

            // Add to ContactsByDate
            var dateKey = contact.CreatedDate.Date;
            if (!ContactsByDate.ContainsKey(dateKey))
            {
                ContactsByDate[dateKey] = new List<Contact>();
            }
            ContactsByDate[dateKey].Add(contact);
        }

        public List<Contact> GetAllContacts()
        {
            return _context.Contacts
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public List<Contact> GetUnreadContacts()
        {
            return _context.Contacts
                .Where(c => !c.IsRead)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public List<Contact> GetContactsByCategory(string category)
        {
            if (ContactsByCategory.ContainsKey(category))
            {
                return ContactsByCategory[category]
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();
            }
            return new List<Contact>();
        }

        public List<Contact> GetRecentContacts(int count = 5)
        {
            return ContactQueue.Reverse().Take(count).ToList();
        }

        public Contact GetContactById(int id)
        {
            return _context.Contacts.FirstOrDefault(c => c.Id == id);
        }

        public bool MarkAsRead(int id)
        {
            var contact = _context.Contacts.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return false;
            }

            contact.IsRead = true;
            _context.SaveChanges();

            return true;
        }

        public bool MarkAsResponded(int id)
        {
            var contact = _context.Contacts.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return false;
            }

            contact.IsResponded = true;
            _context.SaveChanges();

            return true;
        }

        public bool DeleteContact(int id)
        {
            var contact = _context.Contacts.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return false;
            }

            // Store old values for cleanup
            var oldCategory = contact.Category;

            // Remove from database
            _context.Contacts.Remove(contact);
            _context.SaveChanges();

            // Update data structures
            RemoveContactFromDataStructures(contact, oldCategory);

            return true;
        }

        private void RemoveContactFromDataStructures(Contact contact, string oldCategory)
        {
            // Remove from ContactsByCategory
            if (!string.IsNullOrEmpty(oldCategory) && ContactsByCategory.ContainsKey(oldCategory))
            {
                ContactsByCategory[oldCategory].RemoveAll(c => c.Id == contact.Id);
                if (ContactsByCategory[oldCategory].Count == 0)
                {
                    ContactsByCategory.Remove(oldCategory);
                    UniqueCategories.Remove(oldCategory);
                }
            }

            // Remove from ContactQueue
            var queueList = ContactQueue.Where(c => c.Id != contact.Id).ToList();
            ContactQueue.Clear();
            foreach (var c in queueList)
            {
                ContactQueue.Enqueue(c);
            }

            // Remove from ContactsByDate
            var dateKey = contact.CreatedDate.Date;
            if (ContactsByDate.ContainsKey(dateKey))
            {
                ContactsByDate[dateKey].RemoveAll(c => c.Id == contact.Id);
                if (ContactsByDate[dateKey].Count == 0)
                {
                    ContactsByDate.Remove(dateKey);
                }
            }
        }
    }
}