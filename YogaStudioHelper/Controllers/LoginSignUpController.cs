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

            bool valid = db.ValidateUser(email, pass);

            if(valid)
            {
                /*
                IEnumerable<Yoga_User> list = db.getUserByEmail(email);


                int id = list.First().Roles_Id;
                string roleName = db.getRoleName(id);
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
                else 
                {
                   Session["Auth"] = null;
                }
                */

                ViewBag.message = "Valid, Login";
                
                Yoga_User u = db.getUserByEmail(email).Single();
                Session["Uid"] = u.U_Id;

                return RedirectToAction("Homepage", "Home");
            }
            else
            {
                ViewBag.message = "Invalid Login Credentials";
                ViewBag.StickyEmail = email; 
                return View();
            }
           

           
        }

        public ActionResult LogOut()
        {
            Session["Auth"] = null;

            return RedirectToAction("Homepage", "Home");
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(FormCollection collection)
        {
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
    }
}