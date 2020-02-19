using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace YogaStudioHelper.Util
{

    public class EmailSender
    {

        

        public static void sendEmail(string error)
        {
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
                msobj.Subject = error;
                msobj.Body = "You bought a pass!";



                client.Send(msobj);

            }
            catch (Exception e)
            {

            }

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
                         "<a href='http://samsarayogamontreal.gearhostpreview.com/LoginSignUp/ConfirmEmail?email=" + email+"&token="+token+"'>Confirm Email Address</a>";



            client.Send(msobj);
        }


        public static void sendSignUpConfirmationTempPassword(string email, string token, string tempPass)
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
                         "<p>Before you can start purchasing passes and signing up to classes you need to confirm your email address and set a new password</p><br/>" +
                         "<a href='http://samsarayogamontreal.gearhostpreview.com/LoginSignUp/LoginSignUp'>Login here</a>" +
                         "<br/><p>Your temporary password is "+tempPass;



            client.Send(msobj);
        }



        public static void sendPurchaseConfirmation(Yoga_User user, Pass_Log pl, string purchaseType)
        {
            DBMaster db = new DBMaster();

            Class_Passes pass = db.getClassPasse(pl.Pass_Id);

            Promotion p = db.getPromotionByPassId(pl.Pass_Id);

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);

            client.EnableSsl = true;

            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential("SamsaraYogaMontreal@gmail.com", "SamsaraAdminPass");


            MailMessage msobj = new MailMessage();
            
            msobj.To.Add(user.U_Email);
            msobj.From = new MailAddress("SamsaraYogaMontreal@gmail.com");
            msobj.Subject = "Confirmation of "+purchaseType+" Purchase from Samsara Yoga";
            msobj.IsBodyHtml = true;

            if (p == null || p.Promo_End < DateTime.Now.Date)
            {
                decimal tax = ((pass.Pass_Price) * (decimal).15);


                msobj.Body = "<h1 style='color:#557ee6;'>Saṃsāra Yoga</h1><p>Thank you for your recent "+purchaseType.ToLower()+" purchase from Samsara Yoga. Details of this transaction are below:</p><br/>Transaction ID: " + pl.Invoice_Number + "<br/>Transaction Date: " + pl.Date_Purchased + "<br/><br/>Purchased Item: " + pass.Pass_Name + "<br/><br/>‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑<br/><br/>Unit Price: " + pass.Pass_Price.ToString("F") + "<br/>Tax: " + tax.ToString("F") + "<br>Total: " + (tax + pass.Pass_Price).ToString("F") + "$";



                client.Send(msobj);
            }
            else
            {
                if (p.Promo_End.Date > DateTime.Today && p.Num_Classes == 0)
                {
                    decimal discount = decimal.Round((pass.Pass_Price * (decimal)p.Discount * -1), 2);
                    decimal tax = ((pass.Pass_Price + discount) * (decimal).15);

                    msobj.Body = "<h1 style='color:#557ee6;'>Saṃsāra Yoga</h1>" +
                            "<p>Thank you for your recent "+ purchaseType.ToLowerInvariant()+ " purchase from Samsara Yoga. Details of this transaction are below:</p><br/>Transaction ID: " + pl.Invoice_Number + "<br/>Transaction Date: " + pl.Date_Purchased + "<br/><br/>Purchased Item: " + pass.Pass_Name + "<br/>Promotion: "+p.Promo_Desc+" "+(int)(p.Discount*100)+"% Off<br/><br>‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑<br/><br/>Unit Price: " + pass.Pass_Price.ToString("F") + "<br/>Discount: " + discount.ToString("F") +"<br/><br/>Subtotal: "+(discount+pass.Pass_Price).ToString("F")+"<br/>Total: " + (tax + pass.Pass_Price + discount).ToString("F")+"$";



                    client.Send(msobj);
                }
                else
                {
                    
                    decimal tax = ((pass.Pass_Price) * (decimal).15);

                    msobj.Body = "<h1 style='color:#557ee6;'>Saṃsāra Yoga</h1><p>Thank you for your recent "+ purchaseType.ToLowerInvariant()+" purchase from Samsara Yoga. Details of this transaction are below:</p><br/>Transaction ID: " + pl.Invoice_Number + "<br/>Transaction Date: " + pl.Date_Purchased + "<br/><br/>Purchased Item: " + pass.Pass_Name + "<br/>Promotion: "+p.Promo_Desc+" +"+p.Num_Classes+" Passes<br/><br/>‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑<br/><br/>Unit Price: " + pass.Pass_Price.ToString("F") + "<br/>Tax: " + tax.ToString("F") + "<br>Total: " + (tax + pass.Pass_Price).ToString("F") + "$";



                    client.Send(msobj);
                }
            }
        }

        public static void ClassCancelledEmail(List<Yoga_User> list, Schedule schedule)
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

                foreach (Yoga_User u in list)
                {
                msobj.To.Add(u.U_Email);
                }
                msobj.From = new MailAddress("SamsaraYogaMontreal@gmail.com");
                msobj.Subject = "Cancelled Class";
                msobj.IsBodyHtml = true;
                msobj.Body = "<h1 style='color:#557ee6;'>Saṃsāra Yoga</h1>" +
                             "<p>We're sorry to inform you that the " + schedule.Class.Class_Name + " class scheduled for " + schedule.Start_Time.ToString("hh':'mm") + " " + schedule.Class_Date.ToShortDateString() + " has been cancelled.<br/>" +
                             "We've refunded you your class pass.</p><br/>" +
                             "<p>Have a wonderful day,<br/>The Saṃsāra Team</p>";



                client.Send(msobj);
            
        }

        public static void ClassRestoreEmail(List<Yoga_User> list, Schedule schedule)
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

            foreach (Yoga_User u in list)
            {
                msobj.To.Add(u.U_Email);
            }
            msobj.From = new MailAddress("SamsaraYogaMontreal@gmail.com");
            msobj.Subject = "Class Reinstated";
            msobj.IsBodyHtml = true;
            msobj.Body = "<h1 style='color:#557ee6;'>Saṃsāra Yoga</h1>" +
                         "<p>We're writting to inform you that the " + schedule.Class.Class_Name + " class scheduled for " + schedule.Start_Time.ToString("hh':'mm") + " " + schedule.Class_Date.ToShortDateString() + " is no longer cancelled.<br/>" +
                         "If you'd wish to sign up once again please feel free to visit the <a href = 'http://samsarayogamontreal.gearhostpreview.com/' > Saṃsāra site</a></p><br/>" +
                         "<p>Have a wonderful day,<br/>The Saṃsāra Team</p>";



            client.Send(msobj);

        }




        public static void sendContactForm(string email, string subject, string message)
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

            msobj.To.Add("SamsaraYogaMontreal@gmail.com");

            msobj.From = new MailAddress("SamsaraYogaMontreal@gmail.com");
            msobj.Subject = subject;
            msobj.IsBodyHtml = true;
            msobj.Body = message;



            client.Send(msobj);
        }


        public static void sendConfirmation(string email, string subject, string name)
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




            StringBuilder sbEmailBody = new StringBuilder();
            sbEmailBody.Append("Dear " + name + ",<br/><br/>");
            sbEmailBody.Append("Thank you for your email<br/>");
            sbEmailBody.Append("We received you message regarding: " + subject + "<br/>");
            sbEmailBody.Append("We will be back to you as soon as possible");
            sbEmailBody.Append("<br/><br/><br/>");
            sbEmailBody.Append("Sincerely, <br/>");
            sbEmailBody.Append("<b>Samsara Yoga</b>");


            MailMessage msobj = new MailMessage();

            msobj.To.Add(email);

            msobj.From = new MailAddress("SamsaraYogaMontreal@gmail.com");
            msobj.Subject = "Re: Thank you for your email";
            msobj.IsBodyHtml = true;
            msobj.Body = sbEmailBody.ToString();



            client.Send(msobj);
        }



    }

    
}
