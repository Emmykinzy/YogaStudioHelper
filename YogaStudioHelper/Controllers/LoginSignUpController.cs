using Database;
using Scrypt;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YogaStudioHelper.ViewModels;

namespace YogaStudioHelper.Controllers
{
    
    public class LoginSignUpController : Controller
    {
        DBMaster db = new DBMaster();

        ScryptEncoder encoder = new ScryptEncoder();

        [HttpGet]
        public ActionResult LogInSignUp()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LogInSignUp(FormCollection collection)
        {
             
            string email = collection["Email"];
            string pass = collection["Password"];

            bool valid = db.LoginUser(email, pass);



            if (valid)
            {
                
                Yoga_User u = db.getUserByEmail(email).Single();


                int id = u.Roles_Id;
                string roleName = db.getRoleName(id);
                //#+Nta{-- 


                if (id == 1 && u.Active == false || id == 2 && u.Active == false || id == 3 && u.Active == false)
                {
                    Session["Uid"] = u.U_Id;
                    //redirect view to set new password. (replace temporary password)
                    return RedirectToAction("NewPassword","LoginSignUp");
                }


                if(u.Active == true)
                {
                    if (id == 1 || id == 2 || id == 3 || id == 4)
                    {
                        Session["Auth"] = id;
                    }
                }
                else
                {
                    Session["Auth"] = null;
                }



                /*
                if (roleName.Equals("ADMINISTRATOR"))
                {
                    Session["Auth"] = 1;
                }
                else if (roleName.Equals("TEACHER"))
                {
                    Session["Auth"] = 2;
                }
                else if (roleName.Equals("RECEPTIONIST"))
                {
                    Session["Auth"] = 3;
                }
                else if (roleName.Equals("STUDENT"))
                {
                    Session["Auth"] = 4;
                }
                */





                ViewBag.message = "Valid, Login";
                
                Session["Uid"] = u.U_Id;

                return RedirectToAction("Homepage", "Home");
            }
            else
            {
                Yoga_User u = db.getUserByEmail(email).Single();
                if (u.Active == true)
                {
                    ViewBag.message = "Invalid Login Credentials";
                    ViewBag.StickyEmail = email;
                }
                else
                {
                    ViewBag.message = "Account is not Activated";
                    ViewBag.StickyEmail = email;
                }
                return View();
            }
           

           
        }

        [HttpGet]
        public ActionResult NewPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewPassword(FormCollection collection)
        {
            // SHould pass the user id in a safer way so that no one can modify the session id to change the password of someoneelse 
            // hiddenfield instead of session Uid? 

            int userId = Convert.ToInt32(Session["Uid"]);

            var user = db.getUserById(userId); 

            String password1 = collection["password1"].ToString();
            String password2 = collection["password2"].ToString();

            //Validate password enter equal 
            if (!string.Equals(password1, password2))
            {

                TempData["Message"] = "<h5 style=\"color:red;\">Please make sure the two passwords are the same</h5>";
                return View();
            }

            user.U_Password = encoder.Encode(password2);
            user.Active = true;

            db.UpdateUser(user);
            TempData["Message"] = "Your password was successfully updated";


            //return View();
            return RedirectToAction("MessageView","Home");

        }

        public ActionResult LogOut()
        {
            Session["Auth"] = null;
            Session["Uid"] = null;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(FormCollection collection)
        {
            string token = Guid.NewGuid().ToString();            

            string email = collection["Email"];

            

            String password1 = collection["password1"].ToString();

            String password2 = collection["password2"].ToString();


            String firstName = collection["FirstName"].ToString();

            String lastName = collection["LastName"].ToString();

            Yoga_User newUser = new Yoga_User();

            newUser.U_Email = email;
            newUser.U_First_Name = firstName;
            newUser.U_Last_Name = lastName;
            newUser.Roles_Id = 4;
            newUser.Email_Confirmation = token;


            // check if user exist 

            bool validUserExist = db.ValidateUserExist(email);

            if (validUserExist)
            {
                ViewBag.messageSignUp = "This user email is already register";

                ViewBag.StickyUser = newUser;
                
                //ViewBag.set

                return View();

            }

            // Check if both password equals
            if(!string.Equals(password1, password2))
            {
                ViewBag.messageSignUp = "Please make sure the two passwords are the same";

                ViewBag.StickyUser = newUser;

                return View();
            }



            // encode hash the password
            string test = password2; 
            string test2 = encoder.Encode(password2);
            newUser.U_Password = encoder.Encode(password2);
             //newUser.U_Password = password2;

            ViewBag.messageSignUp = "Account created successfully";


            // add user if not already existing 
            try
            {
                //myDB.SaveChanges();
                db.CreateUser(newUser);
                Util.EmailSender.sendSignUpConfirmation(email, token);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                        Console.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
            }



            return View();
        }

        public ActionResult ConfirmEmail()
        {
            string email = Request.QueryString["email"].ToString();
            string token = Request.QueryString["token"].ToString();

            bool valid = db.emailConfirmation(email, token);

            if(valid)
            {
                return View();
            }
            else
            {
                return RedirectToAction("EmailConfirmation", "Error");
            }

            
        }
    }
}