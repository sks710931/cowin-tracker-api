using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CowinTrackerAPI.Models
{
    public class VaccineFee
    {
        public string Vaccine { get; set; }
        public string fee { get; set; }
    }

    public class Session
    {
        public string Session_Id { get; set; }
        public string Date { get; set; }
        public int Available_Capacity { get; set; }
        public int Min_Age_Limit { get; set; }
        public string Vaccine { get; set; }
        public string[] Slots { get; set; }
    }

    public class VaccinationCenter
    { 
        public int Center_Id { get; set; }
        public string Name { get; set; }
        public string Name_1 { get; set; }
        public string Address { get; set; }
        public string Address_1 { get; set; }
        public string State_Name { get; set; }
        public string State_Name_1 { get; set; }
        public string District_Name { get; set; }
        public string District_Name_1 { get; set; }
        public string Block_Name { get; set; }
        public string Block_Name_1 { get; set; }
        public string Pincode { get; set; }
        public int Lat { get; set; }
        public int Long { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Fee_Type { get; set; }
        public List<VaccineFee> Vaccine_Fees { get; set; }
        public List<Session> Sessions { get; set; }

    }

    public class VaccinationCenters
    {
        public List<VaccinationCenter> Centers { get; set; }
    }
}
