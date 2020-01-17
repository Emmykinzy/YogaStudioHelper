using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
//using System.Web.Mvc;
using IdentityModel.Client;
using Scrypt;
using System.Xml.Linq;
using System.Diagnostics;

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

        public Pass_Log getPassLog(DateTime date, int userId)
        {
            IEnumerable<Pass_Log> pls = myDb.Pass_Log.Where(x => x.U_Id == userId);
            Pass_Log pass = (from passlog in pls
                          where passlog.Date_Purchased.Date == date.Date && passlog.Date_Purchased.TimeOfDay == date.TimeOfDay && passlog.U_Id == userId
                          orderby passlog.Pass_Id
                          select passlog).Single();

            return pass;
        }

        public Pass_Log processPurchase(Class_Passes pass, int userId)
        {
            Pass_Log pass_Log = new Pass_Log();

          

            Promotion p = getPromotionByPassId(pass.Pass_Id);
            
            
            pass_Log.Pass_Id = pass.Pass_Id;
            pass_Log.U_Id = userId;
            // num classes 
            int token;
            if (p != null)
            {
                if (p.Num_Classes == 0)
                {
                    token = pass.Pass_Size;
                    pass_Log.Num_Classes = token;

                }
                else
                {
                    token = pass.Pass_Size + (int)p.Num_Classes;
                    pass_Log.Num_Classes = token;

                }

                //Update User Token 
                AddTokens(userId, token);

                // price 
                if (p.Discount == 0)
                {
                    pass_Log.Purchase_Price = decimal.Round(pass.Pass_Price * (decimal)1.15, 2);

                }
                else
                {
                    decimal subtotal = decimal.Round((pass.Pass_Price - (pass.Pass_Price * (decimal)p.Discount)), 2);

                    pass_Log.Purchase_Price = decimal.Round((subtotal * (decimal)1.15), 2);

                }
            }
            else
            {
                token = pass.Pass_Size;
                pass_Log.Num_Classes = token;
                pass_Log.Purchase_Price = decimal.Round(pass.Pass_Price*(decimal)1.15, 2);
            }

            // date 
            DateTime date = DateTime.Now;
            pass_Log.Date_Purchased = date;

            CreatePass_Log(pass_Log);

            string purchaseDateTime = date.ToString("dd/MM/yyyy HH:mm:ss");
            string purchaseDate = date.ToString("ddMMyy");
            string invoice = purchaseDate + userId;


            Pass_Log pl = getPassLog(date, userId);

            string invoiceNumber = invoice + pl.Pass_Log_Id;

            pl.Invoice_Number = Int32.Parse(invoiceNumber);

            myDb.SaveChanges();

            return pl;

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

        public bool LoginUser(string email, string pass)
        {
            try
            {
                bool isValidCustomer;
                var u = myDb.Yoga_User.Where(x => x.U_Email == email).Single();

                if(encoder.Compare(pass, u.U_Password) && u.Active == true)
                {
                   isValidCustomer = true;
                }
                else
                {
                    isValidCustomer = false;
                }
                

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
            } catch
            {
                return false;
            }
        }

        public bool emailConfirmation(string email, string token)
        {
            bool isConfirmed;
            Yoga_User u = getUserByEmail(email).Single();

            if(u.Email_Confirmation == token)
            {
                isConfirmed = true;
                u.Active = true;
            }
            else
            {
                isConfirmed = false;
            }

            myDb.SaveChanges();
            return isConfirmed;
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

        public void AddAvailability(int id, XDocument schedule)
        {
            Yoga_User u = myDb.Yoga_User.Where(x => x.U_Id == id).Single();
            u.Availability = schedule.ToString();
            myDb.SaveChanges();
        }

        public XDocument getAvailability(int id)
        {
            Yoga_User u = myDb.Yoga_User.Where(x => x.U_Id == id).Single();
            XDocument xd = XDocument.Parse(u.Availability);
            return xd;
        }

        public IEnumerable<Yoga_User> getUserAdvancedSearch(string role, string lname, string email)
        {
            IEnumerable<Yoga_User> userList = new List<Yoga_User>();

            if (role == "Select Role" && email == "" && lname == "")
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
            else if (role != "Select Role" && email != "" && lname == "")
            {
                IEnumerable<Yoga_User> list = getUserByRoleName(role);
                var l = list.Where(x => x.Roles_Id == getRoleId(role) && x.U_Email.Contains(email));
                userList = userList.Concat(l);
            }
            else if (role != "Select Role" && email == "" && lname != "")
            {
                IEnumerable<Yoga_User> list = getUserByRoleName(role);
                var l = list.Where(x => x.U_Last_Name.Contains(lname));
                userList = userList.Concat(l);
            }
            else if (role == "Select Role" && email != "" && lname != "")
            {
                IEnumerable<Yoga_User> list = getUserByEmail(email);
                var l = list.Where(x => x.U_Last_Name.Contains(lname));
                userList = userList.Concat(l);
            }
            else if (role != "Select Role" && email == "" && lname == "")
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

        public void UpdateUser(Yoga_User o)
        {
            var n = myDb.Yoga_User.Where(x => x.U_Id == o.U_Id).Single();

            //Yoga_User n = new Yoga_User();

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

        public void DeleteUser(int id)
        {
            var y = myDb.Yoga_User.Where(x => x.U_Id == id).Single();

            myDb.Yoga_User.Remove(y);

            //y.Active = false;

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


        /// <summary>
        ///  // Promotion Get Methods
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>


        public Promotion getPromotion(int id)
        {

            var promo = myDb.Promotions.Where(x => x.Promotion_Id == id).Single();
            return promo;
        }

        public Promotion getPromotionByPassId(int id)
        {
            var promo = myDb.Promotions.Where(x => x.Pass_Id == id).FirstOrDefault();
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

        public void UpdatePromotion(Promotion np)
        {
            var op = myDb.Promotions.Where(x => x.Promotion_Id == np.Promotion_Id).Single();

            //Promotion np = new Promotion();

            op.Promo_Desc = np.Promo_Desc;
            op.Pass_Id = np.Pass_Id;
            op.Promo_End = np.Promo_End;
            op.Num_Classes = np.Num_Classes;
            op.Discount = np.Discount;
            op.Num_Classes = np.Num_Classes;

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


        public bool CheckIfPromoExist(int passId)
        {

            //bool s = Convert.ToBoolean(myDb.Class_Log.Where(x => x.Schedule_Id == schedId && x.U_Id == userId));
            bool s = myDb.Promotions.Any(x => x.Pass_Id == passId);

            // .Any();
            return s;
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


        /// <summary>
        ///     Class Log
        /// </summary>
        /// <returns></returns>


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

        public void CreateClass_Log(int sId, int userId)
        {
            Class_Log newClassLog = new Class_Log();

            newClassLog.Schedule_Id = sId;
            newClassLog.U_Id = userId;
            newClassLog.Log_Status = "SIGNED-UP";

            myDb.Class_Log.Add(newClassLog);
            myDb.SaveChanges();
        }


        // method to test fix null pointer exception 
        public void DeleteAllClass_Log()
        {

            foreach (var log in myDb.Class_Log)
            {
                myDb.Class_Log.Remove(log);


            }

            myDb.SaveChanges();
        }


        public bool CheckIfSignedUp(int schedId, int userId)
        {

            //bool s = Convert.ToBoolean(myDb.Class_Log.Where(x => x.Schedule_Id == schedId && x.U_Id == userId));
            bool s = myDb.Class_Log.Any(x => x.Schedule_Id == schedId && x.U_Id == userId);

            // .Any();
            return s;
        }


        /// <summary>
        ///         //Pass Log Get Methods
        /// </summary>
        /// <returns></returns>


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

        public void DeleteClass_Log(int id)
        {
            var classLog = myDb.Class_Log.Where(x => x.Class_Log_Id == id).Single();

            myDb.Class_Log.Remove(classLog);
            //or.R = false;

            myDb.SaveChanges();

        }




        /// <summary>
        ///             Schedule 
        /// </summary>
        /// <returns></returns>


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
            string startDate = s.Class_Date.ToString("dd/MM/yy");
            string startTime = s.Start_Time.ToString(@"hh\:mm");
            string signedUp = s.Signed_Up.ToString();

            //
            string size = s.Room.Room_Capacity.ToString();

            TimeSpan l = s.Class.Class_Length;
            TimeSpan st = s.Start_Time;

            TimeSpan dur = l + st;

            string duration = dur.ToString(@"hh\:mm");

            desc.Add(id.ToString());
            desc.Add(description);
            desc.Add(name);
            desc.Add(startDate);
            desc.Add(room);
            desc.Add(teacher);
            desc.Add(startTime);
            desc.Add(signedUp);
            desc.Add(duration);
            desc.Add(size);

            s.Start_Time.ToString(@"hh\:mm\:ss");

            return desc;
        }


        public void CreateSchedule(Schedule schedule)
        {
            schedule.Signed_Up = 0;
            myDb.Schedules.Add(schedule);
            myDb.SaveChanges();
        }


        public void UpdateSchedule(Schedule np)
        {
            var sched = myDb.Schedules.Where(x => x.Schedule_Id == np.Schedule_Id).Single();

            //Schedule sched = new Schedule();

            sched.Teacher_Id = np.Teacher_Id;
            sched.Class_Id = np.Class_Id;
            sched.Room_Id = np.Room_Id;
            sched.Start_Time = np.Start_Time;
            sched.Class_Date = np.Class_Date;
            //sched.Signed_Up = np.Signed_Up;
            // sched.Group_Id = np.Group_Id;
            sched.Schedule_Status = np.Schedule_Status;
            sched.Room = np.Room;
            // yoga user?  class log ? class ? 

            myDb.SaveChanges();
        }

        public void DeleteSchedule(int id)
        {
            var or = myDb.Schedules.Where(x => x.Schedule_Id == id).Single();

            myDb.Schedules.Remove(or);
            //or.R = false;

            myDb.SaveChanges();

        }

        public void ArchiveSchedule()
        {

        }



        /// <summary>
        /// Schedule Controller 
        /// </summary>

        public void ScheduleSignUp(int id)
        {
            // How to I know which user I am ? 
            // check teacher session etc? similar? 

            // after check etc and if else 

            var sched = myDb.Schedules.Where(x => x.Schedule_Id == id).Single();

            sched.Signed_Up++;
            
            myDb.SaveChanges();



        }

        /// <summary>
        /// Passes Controller 
        /// </summary>
        /// 

        public void AddTokens(int id, int tokens)
        {

            var n = myDb.Yoga_User.Where(x => x.U_Id == id).Single();

            if (n.U_Tokens == null)
            {
                n.U_Tokens = tokens;
            }
            else
            {
                n.U_Tokens += tokens;
            }



            myDb.SaveChanges();

        }


        public void RemoveToken(int id)
        {

            var n = myDb.Yoga_User.Where(x => x.U_Id == id).Single();

            if (n.U_Tokens != null || n.U_Tokens != 0)
            n.U_Tokens--;
            myDb.SaveChanges();

        }
        /// <summary>
        /// Report Controller 
        /// </summary>
        /// 


        public IEnumerable<Pass_Log> GetSaleReport(DateTime d1, DateTime d2) 
        {

            return myDb.Pass_Log.Where(x => x.Date_Purchased >= d1 && x.Date_Purchased <= d2);

        }


        public IEnumerable<Schedule> GetHoursWorkedReport(DateTime d1, DateTime d2, int userId)
        {

            return myDb.Schedules.Where(x => x.Class_Date >= d1 && x.Class_Date <= d2 && x.Teacher_Id == userId);

        }



    }
}
