using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CowinTrackerAPI.Models;
using Newtonsoft.Json;

namespace CowinTrackerAPI.Helpers
{
    public class Schedule
    {
        
        public static async Task<VaccinationCenters> GetAllVaccinationCentersAsync(int districtId)
        {
            const string URL = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByDistrict";
            VaccinationCenters centers;
            string parameters = "?district_id="+districtId+"&date="+DateTime.Now.ToString("dd-MM-yyyy");
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(parameters).Result;
            if (response.IsSuccessStatusCode)
            {
                
                    string responseBody = await response.Content.ReadAsStringAsync();
                    centers = JsonConvert.DeserializeObject<VaccinationCenters>(responseBody);
                    client.Dispose();
                    return centers;

            }
            
            return null;
        }
        public static List<VaccinationCenter> GetAllVaccinationCentersWithSlotsAvailable(VaccinationCenters vaccinationCenters)
        {
            List<VaccinationCenter> centerWithSlots = new List<VaccinationCenter>();
            foreach (var center in vaccinationCenters.Centers)
            {
                VaccinationCenter tempCenter = new VaccinationCenter();
                List<Session> sessions = new List<Session>();
                foreach (var session in center.Sessions)
                {
                    if (session.Min_Age_Limit == 18 && session.Available_Capacity > 0)
                    {
                        sessions.Add(session);
                    }
                }

                if (sessions.Count > 0)
                {
                    tempCenter = center;
                    center.Sessions = sessions;
                    centerWithSlots.Add(tempCenter);
                }
            }

            return centerWithSlots;
        }
    }

    
}
