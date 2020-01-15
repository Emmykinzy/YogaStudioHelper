using Database;
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



        public static void sendPurchaseConfirmation(Yoga_User user, Pass_Log pl)
        {
            DBMaster db = new DBMaster();

            Class_Passes pass = db.getClassPasse(pl.Pass_Id);
            decimal tax = ((pass.Pass_Price) * (decimal).15);

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);

            client.EnableSsl = true;

            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential("SamsaraYogaMontreal@gmail.com", "SamsaraAdminPass");


            MailMessage msobj = new MailMessage();

            msobj.To.Add(user.U_Email);

            msobj.From = new MailAddress("SamsaraYogaMontreal@gmail.com");
            msobj.Subject = "Confirmation of Digital Purchase from Samsara Yoga";
            msobj.IsBodyHtml = true;
            msobj.Body = "<h1 style='color:#557ee6;'>Saṃsāra Yoga</h1>" +
                         "<p>Thank you for your recent digital purchase from Samsara Yoga. Details of this transaction are below:</p><br/><br/>" +
                         "Transaction ID:"+pl.Invoice_Number+"<br/>"+
                         "Transaction Date:"+pl.Date_Purchased+"<br/><br/>"+
                         "Purchased Item:"+pass.Pass_Name+" Packet<br/><br/><br/>"+
                         "‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑<br/><br/><br/>"+
                         "Unit Price:"+pass.Pass_Price.ToString("F")+"<br/>"+
                         "Tax:"+tax.ToString("F")+"<br>"+
                         "Total:"+(tax+pass.Pass_Price).ToString("F");



            client.Send(msobj);
        }
    }

    
}
