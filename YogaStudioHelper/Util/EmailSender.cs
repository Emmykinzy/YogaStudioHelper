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

                msobj.To.Add("ekjohnson99@gmail.com");

                msobj.From = new MailAddress("SamsaraYogaMontreal@gmail.com");
                msobj.Subject = "This is a Test";                
                msobj.Body = "You bought a pass!";



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

        public static void sendSignUpConfirmation(string email, string token)
        {
            string test = Guid.NewGuid().ToString();

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            //client.EnableSsl = false;

            client.EnableSsl = true;

            client.DeliveryMethod =
                SmtpDeliveryMethod.Network;

            client.UseDefaultCredentials = false;
            //client.Credentials = new System.Net.NetworkCredential("SamsaraYogaMontreal@gmail.com", "SamsaraAdminPass");

            client.Credentials = new System.Net.NetworkCredential("SamsaraYogaMontreal@gmail.com", "SamsaraAdminPass");



            MailMessage msobj = new MailMessage();

            msobj.To.Add(email);

            msobj.From = new MailAddress("SamsaraYogaMontreal@gmail.com");
            msobj.Subject = "Confirm Your Email Address";
            msobj.IsBodyHtml = true;
            msobj.Body = "<h1 style='color:#557ee6;'>Saṃsāra Yoga</h1>" +
                         "<p>Before you can start purchasing passes and signing up to classes you need to confirm your email address</p><br/>" +
                         "<a href='https://localhost:44332/LoginSignUp/ConfirmEmail?email="+email+"&token="+token+"'>Confirm Email Address</a>";



            client.Send(msobj);
        }
    }
}