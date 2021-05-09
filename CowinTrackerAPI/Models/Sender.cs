using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CowinTrackerAPI.Models
{
    public class Sender
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool isExpired { get; set; }
    }
}
