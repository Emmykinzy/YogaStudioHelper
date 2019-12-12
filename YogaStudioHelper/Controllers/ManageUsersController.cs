using System;
using Database;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YogaStudioHelper.Controllers
{
    public class ManageUsersController : Controller
    {
        DBMaster db = new DBMaster();
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
            string role = collection["role"];
            string email = collection["Email"];
            string fname = collection["FirstName"];
            string lname = collection["LastName"];
            Yoga_User y = new Yoga_User();
            y.Roles_Id = db.getRoleId(role);
            y.U_Email = email;
            y.U_First_Name = fname;
            y.U_Last_Name = lname;
            y.Active = false;

            db.CreateUser(y);
            return View();
        }

        public ActionResult UserList()
        {
            IEnumerable<Yoga_User> list = db.getUsers();
            return View(list);
        }

        public ActionResult EditUser()
        {
            return View();
        }

    }
}