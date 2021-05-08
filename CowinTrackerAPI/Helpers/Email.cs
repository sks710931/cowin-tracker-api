using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CowinTrackerAPI.Models;

namespace CowinTrackerAPI.Helpers
{
    public class Email
    {
        static string smtpAddress = "smtp.gmail.com";
        static int portNumber = 587;
        static bool enableSSL = true;
        static string emailFromAddress = "vaccine.tracker.notifications@gmail.com"; //Sender Email Address  
        static string password = "Rachel@123#"; //Sender Password 
        public static void SendEmailNotification(UserRegistration userDetails, List<VaccinationCenter> centers)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(userDetails.Email);
                mail.Subject = "Hi " + userDetails.Name + ", new vaccination slots have opened up!";
                mail.Body = GetEmailBody(userDetails, centers);
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

        public static void SendNoSlotNotification(UserRegistration user)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(user.Email);
                mail.Subject = "Hi " + user.Name + ", currently there are no vaccine slots available!";
                mail.Body = "Hi "+ user.Name +", currently there are no vaccine slots available, we will notify you as soon as slots open up";
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }
        public static string GetEmailBody(UserRegistration userDetails, List<VaccinationCenter> centers)
        {
            string messageBody = "";
            messageBody += "<div style='padding: 30px;'>";
            messageBody += "<p>Hi " + userDetails.Name + "</p>";
            messageBody += "<p>A new Vaccination slot has opened up in "+centers[0].District_Name+"</p>";
            messageBody +=
                "<p>Before the slot fills up, book your appointment immediately through the CoWin website.</p>";
            messageBody += "<h4>Slots available at:</h4>";
            messageBody += "<table style='border-spacing: 0px;margin-bottom: 20px;'>";
            messageBody += "<thead >";
            messageBody += "<tr >";
            messageBody +=
                "<td style='font-weight:bold;border: 1px solid black;border-right: 0px solid transparent; width: 250px; padding: 10px; background-color: azure;'>Address</td>";
            messageBody += "<td style='font-weight:bold;border: 1px solid black;border-right: 0px solid transparent; width: 100px; padding: 10px; background-color: azure;'>Vaccine</td>";
            messageBody += "<td style='font-weight:bold;border: 1px solid black;width: 100px;padding: 10px;background-color: azure;'>Total Slots</td>";
            messageBody += "</tr>"; messageBody += "</thead>";
            messageBody += "<tbody>";
            foreach (VaccinationCenter vaccinationCenter in centers)
            {
                messageBody += " <tr style='border: 1px solid black;'>";
                messageBody += "<td style='padding: 10px;border: 1px solid black;border-right: 0px solid transparent;border-top: 0px solid transparent; width: 250px;'>";
                messageBody += vaccinationCenter.Name + ", " + vaccinationCenter.Address + ", " + vaccinationCenter.Address_1 + ", " + vaccinationCenter.Pincode;
                messageBody += "</td>";
                messageBody += "<td style='padding: 10px;border: 1px solid black;border-right: 0px solid transparent;border-top: 0px solid transparent; width: 250px;'>";
                messageBody += GetAllVaccinesAvailableAtCenter(vaccinationCenter);
                messageBody += "</td>";
                messageBody += "<td style='padding: 10px;border: 1px solid black;border-top: 0px solid transparent; width: 100px;'>";
                messageBody += GetAllVaccinationShots(vaccinationCenter);
                messageBody += "</td>";
                messageBody += "</tr>";
            }
            messageBody += "</tbody>";
            messageBody += "</table>";
            messageBody += "<a href='https://selfregistration.cowin.gov.in/' style='margin-top: 20px;'>Book your slot, now!</a>";
            messageBody += "  </div>";
            return messageBody;
        }

        private static string GetAllVaccinesAvailableAtCenter(VaccinationCenter center)
        {
            string vaccines = "";
            foreach (Session session in center.Sessions)
            {
                if (!vaccines.Contains(session.Vaccine))
                {
                    vaccines += session.Vaccine + ",";
                }
            }
            return vaccines;
        }

        private static string GetAllVaccinationShots(VaccinationCenter center)
        {
            int totalShots = 0;
            foreach (Session session in center.Sessions)
            {
                totalShots += session.Available_Capacity;
            }
            return totalShots.ToString();
        }
    }
}
