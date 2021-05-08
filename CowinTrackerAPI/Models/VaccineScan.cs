using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CowinTrackerAPI.Models
{
    public class VaccineScan
    {
        public int Id { get; set; }
        public int RegistrationId { get; set; }
        public DateTime LastRun { get; set; }
    }
}
