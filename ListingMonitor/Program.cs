using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System;
using System.Xml.Linq;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

internal class Program
{


    private static async Task Main(string[] args)
    {
        string[][] listings = {
            new[]{ "https://evolve.com/vacation-rentals/476188", "NEW! Pet-Friendly Asheville Home w/ Private Yard!", "Thanks to this lovely 2 - bedroom, 2 - bath North Carolina vacation rental, you’ll be able to relax and enjoy all the fun Asheville brings to your vacation." } ,
            new[]{"https://www.vrbo.com/9748448ha","NEW! Pet-Friendly Asheville Home w/ Private Yard!", "Thanks to this lovely 2 - bedroom, 2 - bath North Carolina vacation rental, you’ll be able to relax and enjoy all the fun Asheville brings to your vacation." }
        };
        string message = "";
        string subject = "";
        bool failed = false;
        string s;
        s = "Listing Checks:" + Environment.NewLine;
        Console.WriteLine(s);
    
      
        using (var client = new HttpClient())
        {
            
          
            foreach (var item in listings)
            {

                var response = await client.GetAsync(item[0]);
                s = "Checking listng: " + item[0];
                Console.WriteLine(s);
                message = message + s ;
               
                if (response.IsSuccessStatusCode)
                {
                    var sitehtml = await response.Content.ReadAsStringAsync();
                    if (sitehtml.Contains(item[1]))
                    {
                        s = " found.All Good!";
                        Console.WriteLine(s);
                        message = message + s + Environment.NewLine;
                    }
                    else
                    {
                        s = " not found.  Opps not good check your site.";
                        message = message + s+Environment.NewLine;
                        Console.WriteLine(s);
                        failed = true;
                    }
                }
                else
                {
                    s = "Unable to reach site: " + item[0] + " " + response.StatusCode + " " + response.ReasonPhrase;
                    message = message + s + Environment.NewLine;
                    failed = true;
                    Console.WriteLine(s);
                }
            }
        }
        Console.WriteLine(message);
        subject = (failed) ? "OOPS Problem with Listings" : "Listings all GOOD";
        SendResults("smtp.office365.com", message,subject);

    }
    public static void SendResults(string server, string body,string subject)
    {
        string to = "brady@acm.org";
        string from = "bradyguychambers@outlook.com";
        body = body.Replace("https://", "xxxx").Replace(".", "");
        using (SmtpClient client = new SmtpClient()
        {
            Host = "smtp.office365.com",
            Port = 587,
            UseDefaultCredentials = false, // This require to be before setting Credentials property
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(from, "WarWasp18#"), // you must give a full email address for authentication 
            TargetName = "STARTTLS/smtp.office365.com", // Set to avoid MustIssueStartTlsFirst exception
            EnableSsl = true // Set to avoid secure connection exception
        })
        {

            MailMessage message = new MailMessage()
            {
                From = new MailAddress(from), // sender must be a full email address
                Subject = subject,
                IsBodyHtml = false,
                Body = body,
                BodyEncoding = System.Text.Encoding.UTF8,
                SubjectEncoding = System.Text.Encoding.UTF8,
                

            };
            message.To.Add(to);

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
       

  
    }
}


