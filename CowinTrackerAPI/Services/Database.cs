using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CowinTrackerAPI.Contexts;
using CowinTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CowinTrackerAPI.Services
{
    public class DatabaseService
    {
        private readonly DatabaseContext _context;

        public DatabaseService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task IncreaseEmailCount()
        {
            var res = _context.EmailCount.FirstOrDefault(x => x.Id == 1);
            if (res != null)
            {
                res.EmailSentCount += 1;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Statistics> GetStatistics()
        {
            var emailCount = _context.EmailCount.FirstOrDefault(x => x.Id == 1);
            var userCount = await _context.Set<UserRegistration>().ToListAsync();

                return new Statistics()
                    {TotalNotifications = emailCount.EmailSentCount, TotalRegistrations = userCount.Count};
        }

        public  string GetSender()
        {
            var result = _context.Sender.FirstOrDefault(x => x.isExpired == false);
            if (result != null)
            {
                return result.Email;
            }

            return "vaccine.tracker.notifications@gmail.com";
        }

        public async Task<bool> DisableSender(string sender)
        {
            var result = _context.Sender.FirstOrDefault(x => x.Email == sender);
            result.isExpired = true;
            result.ExpiredAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetSenderStatus()
        {
            var senders = await _context.Set<Sender>().ToListAsync();
            foreach (var sender in senders)
            {
                if (sender.isExpired)
                {
                    var ts = DateTime.Now - sender.ExpiredAt;
                    if (ts.TotalHours > 24)
                    {
                        sender.isExpired = false;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
    public class Statistics
    {
        public int TotalRegistrations { get; set; }
        public int TotalNotifications { get; set; }
    }
}
