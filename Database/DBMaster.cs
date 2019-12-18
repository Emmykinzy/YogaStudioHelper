using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
//using System.Web.Mvc;
using IdentityModel.Client;
using Scrypt;

namespace Database
{
   

    public class DBMaster
    {
         yogadbEntities myDb = new yogadbEntities();
        ScryptEncoder encoder = new ScryptEncoder();



        // USER Methods 

        public IEnumerable<Yoga_User> getUsers()
        {
            return myDb.Yoga_User.ToList();
        }

        public Yoga_User getUserById(int id)
        {
            return myDb.Yoga_User.Where(x => x.U_Id == id).Single();
        }

        public bool ValidateUser(string email, string pass)
        {
            try
            {

                var u = myDb.Yoga_User.Where(x => x.U_Email == email).Single();

                //  var u = myDb.Yoga_User.Where(x => x.U_Email == email && x.U_Password == pass
                //bool isValidCustomer = encoder.Compare(pass, u.U_Password);
                bool isValidCustomer = encoder.Compare(pass, u.U_Password);

                return isValidCustomer; 

            }
            catch
            {
                return false;
            }

        }


        public bool ValidateUserExist(string email)
        {
            try
            {
                var u = myDb.Yoga_User.Where(x => x.U_Email == email).Single();
                return true;
            }catch
            {
                return false;
            }
        }

        public IEnumerable<Yoga_User> getUserByEmail(string email)
        {
            return myDb.Yoga_User.Where(x => x.U_Email.Contains(email));
        }

        public IEnumerable<Yoga_User> getUserByLastName(string lName)
        {
            return myDb.Yoga_User.Where(x => x.U_Last_Name.Contains(lName));
        }

        public IEnumerable<Yoga_User> getUserByFirstName(string fName)
        {
            return myDb.Yoga_User.Where(x => x.U_First_Name.Contains(fName));
        }

        public IEnumerable<Yoga_User> getUserByActive(bool act)
        {
            return myDb.Yoga_User.Where(x => x.Active == act);
        }

        public IEnumerable<Yoga_User> getUserByPhone(string phone)
        {
            return myDb.Yoga_User.Where(x => x.U_Phone.Contains(phone));
        }

        public IEnumerable<Yoga_User> getUserByRoleId(int role)
        {

            return myDb.Yoga_User.Where(x => x.Roles_Id == role);
        }

        public IEnumerable<Yoga_User> getUserByRoleName(string role)
        {
            int r = getRoleId(role);
            return myDb.Yoga_User.Where(x => x.Roles_Id == r);
        }

        //Ben added for schedule
        public List<Yoga_User> getTeacherList()
        {
            //int r = getRoleId(role);
            return myDb.Yoga_User.Where(x => x.Roles_Id == 2).ToList();
        }

        public IEnumerable<Yoga_User> getUserAdvancedSearch(string role, string lname, string email)
        {
            IEnumerable<Yoga_User> userList = new List<Yoga_User>();

            if(role == "Select Role" && email == "" && lname == "")
            {
                IEnumerable<Yoga_User> list = getUsers();
                userList = userList.Concat(list);
            }
            else if (role != "Select Role" && email != "" && lname != "")
            {
                IEnumerable<Yoga_User> list = getUserByRoleName(role);
                var l = list.Where(x => x.Roles_Id == getRoleId(role) && x.U_Email.Contains(email) && x.U_Last_Name.Contains(lname));
                userList = userList.Concat(l);
            }
            else if(role != "Select Role" && email != "" && lname == "")
            {
                IEnumerable<Yoga_User> list = getUserByRoleName(role);
                var l = list.Where(x => x.Roles_Id == getRoleId(role) && x.U_Email.Contains(email));
                userList = userList.Concat(l);
            }
            else if(role != "Select Role" && email == ""  && lname != "")
            {
                IEnumerable<Yoga_User> list = getUserByRoleName(role);
                var l = list.Where(x => x.U_Last_Name.Contains(lname));
                userList = userList.Concat(l);
            }
            else if( role == "Select Role" && email != "" && lname != "")
            {
                IEnumerable<Yoga_User> list = getUserByEmail(email);
                var l = list.Where(x => x.U_Last_Name.Contains(lname));
                userList = userList.Concat(l);
            }
            else if(role != "Select Role" && email == "" && lname == "")
            {
                IEnumerable<Yoga_User> list = getUserByRoleName(role);
                userList = userList.Concat(list);
            }
            else if (role == "Select Role" && email != "" && lname == "")
            {
                IEnumerable<Yoga_User> list = getUserByEmail(email);
                userList = userList.Concat(list);
            }
            else if (role == "Select Role" && email == "" && lname != "")
            {
                IEnumerable<Yoga_User> list = getUserByLastName(lname);
                userList = userList.Concat(list);
            }
 

            return userList;
        }

        // User Create/Update Methods 
        public void CreateUser(Yoga_User y)
        {
            myDb.Yoga_User.Add(y);

            myDb.SaveChanges();

        }

        public void UpdateUser(int id)
        {
           var o = myDb.Yoga_User.Where(x => x.U_Id == id).Single();

            Yoga_User n = new Yoga_User();

            n.Roles_Id = o.Roles_Id;
            n.U_First_Name = o.U_First_Name;
            n.U_Last_Name = o.U_Last_Name;
            n.U_Email = o.U_Email;
            n.U_Password = o.U_Password;
            n.U_Phone = o.U_Phone;
            n.Availability = o.Availability;
            n.U_Birthday = o.U_Birthday;
            n.Active = o.Active;

            myDb.SaveChanges();
        }

        public void ArchiveUser(int id)
        {
            var y = myDb.Yoga_User.Where(x => x.U_Id == id).Single();

            y.Active = false;

            myDb.SaveChanges();
        }





        //  ROLES Methods
        public string getRoleName(int id)
        {
            var r = myDb.Roles.Where(x => x.Roles_Id == id).Single();
            return r.Roles_Name;
        }

        public int getRoleId(string name)
        {
            var r = myDb.Roles.Where(x => x.Roles_Name == name).FirstOrDefault();
            return r.Roles_Id;
        }



        // Room Methods

        public Room getRoom(int id)
        {
            var room = myDb.Rooms.Where(x => x.Room_Id == id).Single();
            return room;
        }
        public List<Room> getRoomList()
        {
            return myDb.Rooms.ToList();
        }
        public IEnumerable<Room> getRooms()
        {
            return myDb.Rooms.ToList();
        }

        public IEnumerable<Room> getRoomsByName(string name)
        {
            return myDb.Rooms.Where(x => x.Room_Name.Contains(name));
        }

        public void CreateRoom(Room r)
        {
            myDb.Rooms.Add(r);
            myDb.SaveChanges();
        }

        public void UpdateRoom(Room newRoom)
        {
            var or = myDb.Rooms.Where(x => x.Room_Id == newRoom.Room_Id).Single();


            or.Room_Name = newRoom.Room_Name;
            or.Room_Capacity = newRoom.Room_Capacity;

            myDb.SaveChanges();
        }

        // Add active in DB to be able to achive 
        public void ArchiveRoom()
        {
  
        }
        public void DeleteRoom(int id)
        {
            var or = myDb.Rooms.Where(x => x.Room_Id == id).Single();

            myDb.Rooms.Remove(or);
            //or.R = false;

            myDb.SaveChanges();
        }




        // Class Get Methods 
        public IEnumerable<Class> getClasses()
        {
            return myDb.Classes.ToList();
        }

        public List<Class> getClassList()
        {
            return myDb.Classes.ToList();
        }


        public Class getClass(int id)
        {

            var oc = myDb.Classes.Where(x => x.Class_Id == id).Single();
            return oc;
        }


        public IEnumerable<Class> getClassesByName(string name)
        {

            return myDb.Classes.Where(x => x.Class_Name.Contains(name));  
            
        }


        public void CreateClass(Class c)
        {
            myDb.Classes.Add(c);
            myDb.SaveChanges();
        }


        //not sure about this one 
        public void UpdateClass(int id)
        {
            var oc = myDb.Classes.Where(x => x.Class_Id == id).Single();

            Class nc = new Class();

            oc.Class_Name = nc.Class_Name;
            oc.Class_Desc = nc.Class_Desc;
            oc.Class_Length = nc.Class_Length;
            oc.Active = nc.Active;

            myDb.SaveChanges();
        }

        public void EditClass(Class newClass)
        {
            var oc = myDb.Classes.Where(x => x.Class_Id == newClass.Class_Id).Single();

            //Class nc = new Class();

            oc.Class_Name = newClass.Class_Name;
            oc.Class_Desc = newClass.Class_Desc;
            oc.Class_Length = newClass.Class_Length;
            oc.Active = newClass.Active;

            myDb.SaveChanges();
        }

        public void DeleteClass(int id)
        {
            var or = myDb.Classes.Where(x => x.Class_Id == id).Single();

            myDb.Classes.Remove(or);
            //or.R = false;

            myDb.SaveChanges();

        }
        public void ArchiveClass()
        {

        }



        //Promotion Get Methods

        public Promotion getPromotion(int id)
        {

            var promo = myDb.Promotions.Where(x => x.Promotion_Id == id).Single();
            return promo; 
        }
        public IEnumerable<Promotion> getPromotions()
        {
            return myDb.Promotions.ToList();
        }

        public IEnumerable<Promotion> getPromotionByDiscount(int dis)
        {
            return myDb.Promotions.Where(x => x.Discount == dis);
        }

        public IEnumerable<Promotion> getPromotionByExtraPasses(int extra)
        {
            return myDb.Promotions.Where(x => x.Num_Classes == extra);
        }

        //Promotion Create/Update Methods

        public void CreatePromotion(Promotion p)
        {
            myDb.Promotions.Add(p);
            myDb.SaveChanges();
        }

        public void UpdatePromotion(int id)
        {
            var op = myDb.Promotions.Where(x => x.Promotion_Id == id).Single();

            Promotion np = new Promotion();

            op.Promo_Desc = np.Promo_Desc;
            op.Pass_Id = np.Pass_Id;
            op.Promo_End = np.Promo_End;
            op.Num_Classes = np.Num_Classes;
            op.Discount = np.Discount;

            myDb.SaveChanges();
        }

        // Archive testing todo see if db has active field
        public void DeletePromotion(int id)
        {
            var or = myDb.Promotions.Where(x => x.Promotion_Id == id).Single();

            myDb.Promotions.Remove(or);


            myDb.SaveChanges();
        }

        public void ArchivePromotion()
        {

        }





        //Class Passes Get Methods


        public Class_Passes getClassPasse(int id)
        {
            //return myDb.Class_Passes.ToList();
            var class_passe = myDb.Class_Passes.Where(x => x.Pass_Id == id).Single();
            return class_passe;
        }
        public List<Class_Passes> getClassPasses()
        {
            return myDb.Class_Passes.ToList();
        }

        public IEnumerable<Class_Passes> getClassPassByName(string name)
        {
            return myDb.Class_Passes.Where(x => x.Pass_Name.Contains(name));
        }

        public IEnumerable<Class_Passes> getClassPassBySize(int size)
        {
            return myDb.Class_Passes.Where(x => x.Pass_Size == size);
        }

        public IEnumerable<Class_Passes> getClassPassByPrice(decimal price)
        {
            return myDb.Class_Passes.Where(x => x.Pass_Price == price);
        }

        public void CreateClassPass(Class_Passes cp)
        {
            myDb.Class_Passes.Add(cp);
            myDb.SaveChanges();
        }

        public void UpdateClassPass(Class_Passes pass)
        {


            var ocp = myDb.Class_Passes.Where(x => x.Pass_Id == pass.Pass_Id).Single();

            Class_Passes ncp = new Class_Passes();

            ocp.Pass_Name = pass.Pass_Name;
            ocp.Pass_Size = pass.Pass_Size;
            ocp.Pass_Price = pass.Pass_Price;
            ocp.Active = pass.Active;

            myDb.SaveChanges();
        }

        public void DeleteClassPass(int id)
        {
            var or = myDb.Class_Passes.Where(x => x.Pass_Id == id).Single();

            myDb.Class_Passes.Remove(or);
            //or.R = false;

            myDb.SaveChanges();

        }
        public void ArchiveClassPass()
        {

        }





        //Class Log Get Methods
        public IEnumerable<Class_Log> GetClass_Logs()
        {
            return myDb.Class_Log.ToList();
        }

        public IEnumerable<Class_Log> GetClass_LogsByScheduleId(int id)
        {
            return myDb.Class_Log.Where(x => x.Schedule_Id == id);
        }

        public IEnumerable<Class_Log> GetClass_LogsByUId(int id)
        {
            return myDb.Class_Log.Where(x => x.U_Id == id);
        }

        //Class Log Create Method

        public void CreateClass_Log(Class_Log cl)
        {
            myDb.Class_Log.Add(cl);
            myDb.SaveChanges();
        }




        //Pass Log Get Methods

        public IEnumerable<Pass_Log> getPass_Logs()
        {
            return myDb.Pass_Log.ToList();
        }

        public IEnumerable<Pass_Log> getPass_LogsByPassId(int id)
        {
            return myDb.Pass_Log.Where(x => x.Pass_Id == id);
        }

        public IEnumerable<Pass_Log> getPass_LogsByUId(int id)
        {
            return myDb.Pass_Log.Where(x => x.U_Id == id);
        }

        public IEnumerable<Pass_Log> getPass_LogsByTimeframe(DateTime start, DateTime end)
        {
            return myDb.Pass_Log.Where(x => x.Date_Purchased >= start && x.Date_Purchased <= end);
        }

        //Pass Log Create Method

        public void CreatePass_Log(Pass_Log pl)
        {
            myDb.Pass_Log.Add(pl);
            myDb.SaveChanges();
        }




        //Schedule Get Methods
        public IEnumerable<Schedule> getSchedules()
        {
            return myDb.Schedules.ToList();
        }

        public Schedule getScheduleById(int id)
        {
            return myDb.Schedules.Where(x => x.Schedule_Id == id).Single();
        }

        public IEnumerable<Schedule> getSchedulesNext7Days()
        {
            DateTime today = DateTime.Today;
            DateTime week = today.AddDays(7);
            return myDb.Schedules.Where(x => x.Class_Date >= today && x.Class_Date <= week);
        }

        public IEnumerable<string> getSchedulesInfo(int id)
        {
            Schedule s = getScheduleById(id);
            List<string> desc = new List<string>();

            string description = s.Class.Class_Desc;
            string name = s.Class.Class_Name;
            string room = s.Room.Room_Name;
            string teacher = s.Yoga_User.U_First_Name + " " + s.Yoga_User.U_Last_Name;
            //string startDate = s.Class_Date. ToString("dd/MM/yy");
            //string startTime = s.Start_Time.ToString(@"hh\:mm");
            string signedUp = s.Signed_Up.ToString();

            //TimeSpan l = s.Class.Class_Length;
            //TimeSpan st = s.Start_Time;

            //TimeSpan dur = l + st;

            //string duration = dur.ToString(@"hh\:mm");

            desc.Add(id.ToString());
            desc.Add(description);
            desc.Add(name);
            //desc.Add(startDate);
            desc.Add(room);
            desc.Add(teacher);
            //desc.Add(startTime);
            desc.Add(signedUp);
            //desc.Add(duration);

            //s.Start_Time.ToString(@"hh\:mm\:ss");

            return desc;
        }
    }
}
