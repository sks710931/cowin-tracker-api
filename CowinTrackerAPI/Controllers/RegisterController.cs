using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CowinTrackerAPI.Contexts;
using CowinTrackerAPI.Helpers;
using CowinTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CowinTrackerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public RegisterController(DatabaseContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("/test")]
        public ActionResult GetTest()
        {
            return Ok("Test SuccessFul");
        }

        [HttpPost]
        [Route("user-registration")]
        public async Task<ActionResult> RegisterUserAsync([FromBody] UserRegistration details)
        {
            var result = _context.UserRegistration.Add(details);
            await _context.SaveChangesAsync();
            var centers = await Schedule.GetAllVaccinationCentersAsync(result.Entity.DistrictId);
            var centersWithSlots = Schedule.GetAllVaccinationCentersWithSlotsAvailable(centers);
            if (centersWithSlots.Count > 0)
            {
                Email.SendEmailNotification(result.Entity, centersWithSlots);
            }
            else
            {
                Email.SendNoSlotNotification(result.Entity);
            }

            var user = _context.UserRegistration.FirstOrDefault(x => x.Email == result.Entity.Email);
            var scanResult = _context.VaccineScan.Add(new VaccineScan()
                {Id = 0, RegistrationId = user.Id, LastRun = DateTime.Now});
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost]
        [Route("run-vaccine-scan")]
        public async Task<ActionResult> RunVaccineScan()
        {
            //get All previous scans 
            var scans = await _context.Set<VaccineScan>().ToListAsync();

            // run scan for each entries
            foreach (var scan in scans)
            {
                
                // get scan user details
                var details = _context.UserRegistration.FirstOrDefault(x => x.Id == scan.RegistrationId);
                if (details != null)
                {
                    //check whether to run the scan or not
                    var timespan = DateTime.Now - scan.LastRun;
                    if (timespan.TotalMinutes >= 5)
                    {
                        var centers = await Schedule.GetAllVaccinationCentersAsync(details.DistrictId);
                        var centersWithSlots = Schedule.GetAllVaccinationCentersWithSlotsAvailable(centers);
                        if (centersWithSlots.Count > 0)
                        {
                            Email.SendEmailNotification(details, centersWithSlots);
                            
                        }
                        scan.LastRun = DateTime.Now;

                    }
                    
                }
                
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
