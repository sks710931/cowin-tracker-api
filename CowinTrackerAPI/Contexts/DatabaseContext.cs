using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CowinTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CowinTrackerAPI.Contexts
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<UserRegistration> UserRegistration { get; set; }
        public DbSet<VaccineScan> VaccineScan { get; set; }
    }
}
