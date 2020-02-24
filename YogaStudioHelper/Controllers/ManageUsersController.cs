using System;
using Database;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Web.Security;
using Scrypt;

namespace YogaStudioHelper.Controllers
{
    [Filters.AuthorizeAdmin]
    public class ManageUsersController : Controller
    {
        DBMaster db = new DBMaster();
        ScryptEncoder encoder = new ScryptEncoder();


        // GET: ManageUsers

        [HttpGet]
        public ActionResult FindUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FindUser(FormCollection collection)
        {
            string email = collection["Email"];
            string active = collection["Active"];
            IEnumerable<Yoga_User> userList = db.getUserByEmail(email);
            
            if (userList.Count() == 0)
            {
                ViewBag.FindClassMessage = "No users with an email containing " + email + " was found";
                return View();
            }
            else
            {
                if(active == "true")
                {
                    IEnumerable<Yoga_User> l = userList.Where(x => x.Active == true);
                    TempData["userList"] = l;
                    return RedirectToAction("FindUserList");
                }
                else
                {
                    TempData["userList"] = userList;
                    return RedirectToAction("FindUserList");
                }
               
            }
            
           
        }

        public ActionResult FindUserList()
        {
            if (TempData["userList"] != null)
            {
                IEnumerable<Yoga_User> c = TempData["userList"] as IEnumerable<Yoga_User>;
                return View(c);
            }
            else
            {
                IEnumerable<Yoga_User> c = TempData["List"] as IEnumerable<Yoga_User>;

                return View(c);
            }
        }

        [HttpGet]
        public ActionResult FindUserAdvanced()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FindUserAdvanced(FormCollection collection)
        {
            string role = collection["role"];
            string email = collection["Email"];
            string lname = collection["LastName"];
           

            IEnumerable<Yoga_User> list = db.getUserAdvancedSearch(role, lname, email);

            if (list.Count() == 0)
            {
                ViewBag.FindClassMessage = "No users were found";
                return View();
            }
            else
            {
                TempData["userList"] = list;
                return RedirectToAction("FindUserListAdvanced");
            }
            
        }

        public ActionResult FindUserListAdvanced()
        {
            if (TempData["userList"] != null)
            {
                IEnumerable<Yoga_User> c = TempData["userList"] as IEnumerable<Yoga_User>;
                return View(c);
            }
            else
            {
                IEnumerable<Yoga_User> c = TempData["List"] as IEnumerable<Yoga_User>;

                return View(c);
            }
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(FormCollection collection)
        {
            int role = Convert.ToInt32(collection["role"]);
            string email = collection["Email"];
            string fname = collection["FirstName"];
            string lname = collection["LastName"];
            string pass = collection["Password"];

            //
            string phone = collection["Phone"];
            DateTime birthday = Convert.ToDateTime(collection["Birthday"]);



            Yoga_User y = new Yoga_User();
            //y.Roles_Id = db.getRoleId(role);
            y.Roles_Id = role;

            y.U_Email = email;
            y.U_First_Name = fname;
            y.U_Last_Name = lname;

            y.U_Phone = phone;
            y.U_Birthday = birthday;


            // will do false so that the user need to update the temporary password
            y.Active = false;

            //  Generate temporary password and send confirmation email 

            String tempPassword = Membership.GeneratePassword(8, 2);
            y.U_Password = encoder.Encode(pass);
            
            //string token = Guid.NewGuid().ToString();
            //Util.EmailSender.sendSignUpConfirmationTempPassword(email, token, tempPassword);



            // If teacher 
            if (role == 2)
            {
                // "N/A" Me
                XDocument availabilities = new XDocument
                (
                new XElement("Root",
                       new XElement("Sunday",
                       new XElement("Start", "N/A"),
                       new XElement("End", "N/A")),
                   new XElement("Monday",
                       new XElement("Start", "N/A"),
                       new XElement("End", "N/A")),
                   new XElement("Tuesday",
                       new XElement("Start", "N/A"),
                       new XElement("End", "N/A")),
                   new XElement("Wednesday",
                       new XElement("Start", "N/A"),
                       new XElement("End", "N/A")),
                   new XElement("Thursday",
                       new XElement("Start", "N/A"),
                       new XElement("End", "N/A")),
                   new XElement("Friday",
                       new XElement("Start", "N/A"),
                       new XElement("End", "N/A")),
                   new XElement("Saturday",
                       new XElement("Start", "N/A"),
                       new XElement("End", "N/A"))
                ));

                y.Availability = availabilities.ToString();
            }

            db.CreateUser(y);
            return RedirectToAction("UserList");
        }

        [HttpGet]
        public ActionResult UserList()
        {
            IEnumerable<Yoga_User> list = db.getUsers();
            IEnumerable<Yoga_User> orderedList = (from user in list
                                                  orderby user.U_Last_Name
                                                  orderby user.Roles_Id
                                                  select user);
            return View(orderedList.Take(10));
        }

        [HttpPost]
        public ActionResult UserList(FormCollection form)
        {
            IEnumerable<Yoga_User> list = db.getUsers();
            IEnumerable<Yoga_User> orderedList = (from user in list
                                                  orderby user.U_Last_Name
                                                  orderby user.Roles_Id
                                                  select user);
            IEnumerable<Yoga_User> newList;
            if (form["back"] == null)
            {
                if (form["position"] == null)
                {
                    newList = orderedList.Skip(1 * 10).Take(10);
                    TempData["position"] = 2;
                }
                else
                {
                    int position = Int32.Parse(form["position"]);
                    newList = orderedList.Skip(position * 10).Take(10);
                    TempData["position"] = position + 1;
                }

            }
            else
            {

                int position = Int32.Parse(form["position"]);
                newList = orderedList.Skip(position - 1 * 10).Take(10);
                TempData["position"] = position - 1;

            }


            return View(newList);
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            var user = db.getUserById(id);

            ViewBag.StickyUser = user;

            return View();
        }

        [HttpPost]
        public ActionResult EditUser(FormCollection collection)
        {

            int id = (int)TempData["EditUserId"];

            var y = db.getUserById(id);


            int role = Convert.ToInt32(collection["role"]);
            string email = collection["Email"];
            string fname = collection["FirstName"];
            string lname = collection["LastName"];
            string phone = collection["Phone"];
            //string birthday = collection["Birthday"];
            DateTime birthday = Convert.ToDateTime(collection["Birthday"]);

            //Yoga_User y = new Yoga_User();
            y.Roles_Id = role;
            y.U_Email = email;
            y.U_First_Name = fname;
            y.U_Last_Name = lname;
            y.U_Phone = phone;
            y.U_Birthday = birthday;

            if(collection["active"] == null)
            {
                y.Active = false;
            }
            else
            {
                y.Active = true;
            }
            
            // see for password


            //update db method
            db.UpdateUser(y);

            //ViewBag.StickyUser = user;

            return RedirectToAction("UserList");
        }

        public ActionResult ArchiveUser(int id)
        {
            //SHould implement archive instead
            db.DeleteUser(id);
            // should use delete method in futur instead 

            return RedirectToAction("UserList");
        }


        //



        [HttpGet]
        public ActionResult EditPassword(int id)
        {
            ViewBag.UserId = id;

            return View();
        }

        [HttpPost]
        public ActionResult EditPassword(FormCollection collection)
        {
            // SHould pass the user id in a safer way so that no one can modify the session id to change the password of someoneelse 
            // hiddenfield instead of session Uid? 

            int userId = (int)TempData["EditUserId"];

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
            //user.Active = true;

            db.UpdateUser(user);
            TempData["Message"] = "Your password was updated successfully.";


            //return View();
            return RedirectToAction("MessageView", "Home");

        }

    }
}