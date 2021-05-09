using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CowinTrackerAPI.Contexts;
using CowinTrackerAPI.Helpers;
using CowinTrackerAPI.Models;
using CowinTrackerAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CowinTrackerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly DatabaseService _dbService;

        public RegisterController(DatabaseContext context, DatabaseService dbService)
        {
            _context = context;
            _dbService = dbService;
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
            try
            {
                if (_context.UserRegistration.FirstOrDefault(x => x.Email == details.Email) == null)
                {
                    var result = _context.UserRegistration.Add(details);
                    await _context.SaveChangesAsync();
                    var centers = await Schedule.GetAllVaccinationCentersAsync(result.Entity.DistrictId);
                    var centersWithSlots = Schedule.GetAllVaccinationCentersWithSlotsAvailable(centers);
                    try
                    {
                        if (centersWithSlots.Count > 0)
                        {
                            Email.SendEmailNotification(result.Entity, centersWithSlots, _dbService.GetSender());
                            await _dbService.IncreaseEmailCount();
                        }
                        else
                        {
                            Email.SendNoSlotNotification(result.Entity, _dbService.GetSender());
                            await _dbService.IncreaseEmailCount();
                        }
                    }
                    catch (Exception e)
                    {
                        var sender = _dbService.GetSender();
                        await _dbService.DisableSender(sender);
                    }

                    var user = _context.UserRegistration.FirstOrDefault(x => x.Email == result.Entity.Email);
                    var scanResult = await _context.VaccineScan.AddAsync(new VaccineScan()
                        { Id = 0, RegistrationId = user.Id, LastRun = DateTime.Now });
                    await _context.SaveChangesAsync();
                    return Ok(user);
                }
            }
            catch (Exception e)
            {
                
                return StatusCode(500, new Error() { Status = 208, Message = "Internal Server Error! Please try again later." });
            }

            return StatusCode(208, new Error() { Status = 208, Message = "Your email is already registered!" });
        }

        [HttpGet]
        [Route("unsubscribe/{email}")]
        public async Task<ActionResult> Unsubscribe(string email)
        {
            var detail = _context.UserRegistration.FirstOrDefault(x => x.Email == email);
            if (detail != null)
            {
                var scan = _context.VaccineScan.FirstOrDefault(x => x.RegistrationId == detail.Id);
                if (scan != null)
                {
                    _context.Set<VaccineScan>().Remove(scan);
                }

                _context.Set<UserRegistration>().Remove(detail);
                await _context.SaveChangesAsync();
            }

            return Ok("Thanks for using our vaccine tracker. You have been unsubscribed from email notifications!");
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
                            try
                            {
                                Email.SendEmailNotification(details, centersWithSlots, _dbService.GetSender());
                                await _dbService.IncreaseEmailCount();
                            }
                            catch (Exception e)
                            {
                                var sender = _dbService.GetSender();
                                await _dbService.DisableSender(sender);
                            }

                        }
                        scan.LastRun = DateTime.Now;

                    }
                    
                }
                
            }

            await _context.SaveChangesAsync();
            await _dbService.ResetSenderStatus();
            return Ok();
        }

        [HttpGet]
        [Route("statistics")]
        public async Task<ActionResult> GetStatistics()
        {
            return Ok(await _dbService.GetStatistics());
        }
    }

    
    public class Error
    {
        public int Status { get; set; }
        public string Message { get; set; }

    }
}
