using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace YogaStudioHelper.Util
{
    public class EmailSender
    {


        public static string sendEmail()
        {
            String message; 
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                //client.EnableSsl = false;

                client.EnableSsl = true;

                client.DeliveryMethod =
                    SmtpDeliveryMethod.Network;

                client.UseDefaultCredentials = false;
                //client.Credentials = new System.Net.NetworkCredential("SamsaraYogaMontreal@gmail.com", "SamsaraAdminPass");

                client.Credentials = new System.Net.NetworkCredential("SamsaraYogaMontreal@gmail.com", "SamsaraAdminPass");



                MailMessage msobj = new MailMessage();

                msobj.To.Add("benlegend9@gmail.com");

                msobj.From = new MailAddress("SamsaraYogaMontreal@gmail.com");
                msobj.Subject = "subject here";
                msobj.Body = "text body";



                client.Send(msobj);

                 message = "Email sent successfully";
                //Response.Write(message);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                 message = "Error sending the email";

                //Response.Write(message + "  " + e);
                Console.WriteLine(message);

            }
            return message;

        }
    }
}