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

                ViewBag.message = "Valid, Login";


                return RedirectToAction("Homepage", "Home");
            }
            else
            {
                ViewBag.message = "Invalid Login Credentials";
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

            // check if user exist 

            bool validUserExist = db.ValidateUserExist(email);

            if (validUserExist)
            {
                ViewBag.messageSignUp = "This user email is already register";
                return View();

            }

            // Check both password 
            if(!string.Equals(password1, password2))
            {
                ViewBag.messageSignUp = "Please make sure the two passwords are the same";
                return View();
            }


            // add user if not already existing 

            Yoga_User newUser = new Yoga_User();

            newUser.U_Email = email;
            newUser.U_First_Name = firstName;
            newUser.U_Last_Name = lastName;
            newUser.Roles_Id = 4;

            // todo validate 2 password equals 
            // encode hash the password
            newUser.U_Password = encoder.Encode(password2);

            //newUser.U_Password = password2;

            ViewBag.messageSignUp = "Account created successfully";


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


            
            //CreateUser


            return View();
        }
    }
}