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
using YogaStudioHelper.Models;
using Database.ModelsDB;
using System.Collections;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
//using YogaStudioHelper.Models;

namespace Database
{
   

    public class DBMaster
    {
        public yogadbEntities myDb = new yogadbEntities();
        ScryptEncoder encoder = new ScryptEncoder();


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

        public Pass_Log processPurchase(Class_Passes pass, int userId, string purchaseType)
        {
            Pass_Log pass_Log = new Pass_Log();

          

            Promotion p = getPromotionByPassId(pass.Pass_Id);
            
            
            pass_Log.Pass_Id = pass.Pass_Id;
            pass_Log.U_Id = userId;
            pass_Log.Purchase_Method = purchaseType;

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

            AddTokens(userId, token);
            DateTime date = DateTime.Now;
            pass_Log.Date_Purchased = date;
            

            CreatePass_Log(pass_Log);

            string purchaseDateTime = date.ToString("dd/MM/yyyy HH:mm:ss");
            string purchaseDate = date.ToString("ddMMyy");

            Pass_Log pl = getPassLog(date, userId);

            string invoiceNumber = userId.ToString() + pl.Pass_Log_Id;

            pl.Invoice_Number = Int32.Parse(invoiceNumber);

            myDb.SaveChanges();

            return pl;

        }

        public bool ValidateUser(string email, string pass)
        {
            try
            {

                var u = myDb.Yoga_User.Where(x => x.U_Email == email).Single();

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

                if (encoder.Compare(pass, u.U_Password))
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

        public IEnumerable<Yoga_User> getUserByPartialEmail(string email)
        {
            return myDb.Yoga_User.Where(x => x.U_Email.Contains(email));
        }

        public IEnumerable<Yoga_User> getUserByEmail(string email)
        {
            return myDb.Yoga_User.Where(x => x.U_Email.Equals(email));
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

        public List<Yoga_User> getTeacherList()
        {
            return myDb.Yoga_User.Where(x => x.Roles_Id == 2 && x.Active == true).ToList();
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
                IEnumerable<Yoga_User> list = getUserByPartialEmail(email);
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
                IEnumerable<Yoga_User> list = getUserByPartialEmail(email);
                userList = userList.Concat(list);
            }
            else if (role == "Select Role" && email == "" && lname != "")
            {
                IEnumerable<Yoga_User> list = getUserByLastName(lname);
                userList = userList.Concat(list);
            }


            return userList;
        }

        public void CreateUser(Yoga_User y)
        {
            myDb.Yoga_User.Add(y);

            myDb.SaveChanges();

        }

        public void UpdateUser(Yoga_User o)
        {
            var n = myDb.Yoga_User.Where(x => x.U_Id == o.U_Id).Single();

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

        public void ReActivateUser(int id)
        {
            var y = myDb.Yoga_User.Where(x => x.U_Id == id).Single();

            y.Active = true;

            myDb.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var y = myDb.Yoga_User.Where(x => x.U_Id == id).Single();

            myDb.Yoga_User.Remove(y);

            //y.Active = false;

            myDb.SaveChanges();
        }

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

        public Room getRoom(int id)
        {
            var room = myDb.Rooms.Where(x => x.Room_Id == id).Single();
            return room;
        }
        public List<Room> getRoomList()
        {
            return myDb.Rooms.ToList();
        }
        public List<Room> getRoomActiveList()
        {
            return myDb.Rooms.Where(x =>x.Active == true).ToList();
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
            or.Active = newRoom.Active;

            myDb.SaveChanges();
        }

        public void ArchiveRoom(int id)
        {

            var room = myDb.Rooms.Where(x => x.Room_Id == id).Single();

            var sched = myDb.Schedules.FirstOrDefault
                (e => e.Room_Id == id);

            if(sched == null)
            {
                myDb.Rooms.Remove(room);
            }
            else
            {
                room.Active = false;
            }

            
            myDb.SaveChanges();
        }

        public void ReactivateRoom(int id)
        {

            var room = myDb.Rooms.Where(x => x.Room_Id == id).Single();

            var sched = myDb.Schedules.FirstOrDefault
                (e => e.Room_Id == id);

            if (sched == null)
            {
                myDb.Rooms.Remove(room);
            }
            else
            {
                room.Active = true;
            }


            myDb.SaveChanges();
        }
        public void DeleteRoom(int id)
        {
            var or = myDb.Rooms.Where(x => x.Room_Id == id).Single();

            myDb.Rooms.Remove(or);

            myDb.SaveChanges();
        }


        public IEnumerable<Class> getClasses()
        {
            return myDb.Classes.ToList();
        }

        public List<Class> getClassList()
        {
            return myDb.Classes.Where(x => x.Active == true).ToList();
        }

        public List<Class> getClassActiveList()
        {
            return myDb.Classes.Where(x => x.Active == true).ToList();
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

            myDb.SaveChanges();

        }
        public void ArchiveClass(int id)
        {
            var classe = myDb.Classes.Where(x => x.Class_Id == id).Single();
            int idtest = id;
            var sched = myDb.Schedules.Where(x => x.Class_Id == id).FirstOrDefault();

            if(sched == null)
            {
                myDb.Classes.Remove(classe);
            }
            else
            {
                classe.Active = false;
            }
  
            myDb.SaveChanges();
        }

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
            return myDb.Promotions.OrderBy(x => x.Promo_End).ToList();
        }

        public IEnumerable<Promotion> getPromotionByDiscount(int dis)
        {
            return myDb.Promotions.Where(x => x.Discount == dis);
        }

        public IEnumerable<Promotion> getPromotionByExtraPasses(int extra)
        {
            return myDb.Promotions.Where(x => x.Num_Classes == extra);
        }

        public void CreatePromotion(Promotion p)
        {
            myDb.Promotions.Add(p);
            myDb.SaveChanges();
        }

        public void UpdatePromotion(Promotion np)
        {
            var op = myDb.Promotions.Where(x => x.Promotion_Id == np.Promotion_Id).Single();

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

        public bool CheckIfPromoExist(int passId)
        {
            bool s = myDb.Promotions.Any(x => x.Pass_Id == passId);
            
            return s;
        }

        public bool CheckIfActivePromoExist(int passId)
        {
            bool s = myDb.Promotions.Any(x => x.Pass_Id == passId && x.Promo_End < DateTime.Now.Date);

            return s;
        }





        //Class Passes Get Methods


        public Class_Passes getClassPasse(int id)
        {
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

            myDb.SaveChanges();

        }
        public void ArchiveClassPass(int id)
        {
            var classpass = myDb.Class_Passes.Where(x => x.Pass_Id == id).Single();

            var classlog = myDb.Pass_Log.Where(x => x.Pass_Id == id).FirstOrDefault();

            var promotion = myDb.Promotions.Where(x => x.Promotion_Id == id).FirstOrDefault();

            // check if the classpass is begin reference to see if can delete instead of archiving 
            if (classlog == null && promotion == null)
            {
                myDb.Class_Passes.Remove(classpass);
            }
            else
            {
                classpass.Active = false;
            }

            myDb.SaveChanges();
        }

        public IEnumerable<Class_Log> GetClass_Logs()
        {
            return myDb.Class_Log.ToList();
        }

        public Class_Log GetClass_LogsByUidAndSid(int uid, int sid)
        {
            Class_Log cl = myDb.Class_Log.Where(x => x.U_Id == uid && x.Schedule_Id == sid).FirstOrDefault();
            return cl;
        }
        public int getSignedUp(int scheduleId)
        {
            IEnumerable<Class_Log> cl = myDb.Class_Log.Where(x => x.Schedule_Id == scheduleId);
            return cl.Count();
        }

        public IEnumerable<Class_Log> GetClass_LogsByScheduleId(int id)
        {
            return myDb.Class_Log.Where(x => x.Schedule_Id == id);
        }

        public IEnumerable<Class_Log> GetClass_LogsByUId(int id)
        {
            return myDb.Class_Log.Where(x => x.U_Id == id);
        }

        public void CreateClass_Log(int sId, int userId)
        {
            Class_Log newClassLog = new Class_Log();

            newClassLog.Schedule_Id = sId;
            newClassLog.U_Id = userId;
            newClassLog.Log_Status = "MISSED";

            myDb.Class_Log.Add(newClassLog);
            myDb.SaveChanges();
        }

        public void changeClass_LogStatus(int userId, int scheduleId, string attendance)
        {
            Class_Log cl = myDb.Class_Log.Where(x => x.U_Id == userId && x.Schedule_Id == scheduleId).SingleOrDefault();
            cl.Log_Status = attendance;
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
            bool s = myDb.Class_Log.Any(x => x.Schedule_Id == schedId && x.U_Id == userId);

            return s;
        }

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

        public IEnumerable<Schedule> getSchedules()
        {
            return myDb.Schedules.ToList();
        }

        public List<ScheduleListViewModel> getScheduleViewModelList()
        {

            List<ScheduleListViewModel> list = new List<ScheduleListViewModel>();
            var scheduleList = myDb.Schedules.ToList();

            foreach(var sched in scheduleList) {

                var schedVM = new ScheduleListViewModel();

                var teacher = getUserById(sched.Teacher_Id);
                var classe = getClass(sched.Class_Id);

                schedVM.Schedule_Id = sched.Schedule_Id;
                schedVM.U_First_Name = teacher.U_First_Name;
                schedVM.U_Last_Name = teacher.U_Last_Name;
                schedVM.Class_Name = classe.Class_Name;
                schedVM.Start_Time = sched.Start_Time;
                schedVM.Class_Date = sched.Class_Date;
                schedVM.Signed_Up = sched.Signed_Up;
                schedVM.Group_Id = sched.Group_Id;
                schedVM.Room_Name = sched.Room.Room_Name;
                schedVM.Schedule_Status = sched.Schedule_Status;

                list.Add(schedVM);
            }

            return list;
        }

        public IEnumerable<Schedule> getScheduleByRoomAndDay(int roomId, DateTime day)
        {
            IEnumerable<Schedule> sList = (from schedule in myDb.Schedules
                     where schedule.Class_Date == day && schedule.Room_Id == roomId
                     select schedule);
            return sList;
        }

        public bool isScheduleActive(int id)
        {
            Schedule schedule = myDb.Schedules.Where(x => x.Schedule_Id == id).FirstOrDefault();
            if(schedule.Schedule_Status == "ACTIVE")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public IEnumerable<Schedule> getScheduleByTeacherAndDay(int uid, DateTime day)
        {
            IEnumerable<Schedule> sList = (from schedule in myDb.Schedules
                                           where schedule.Class_Date == day && schedule.Teacher_Id == uid
                                           select schedule);
            return sList;
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

        public IEnumerable<Schedule> getSchedulesNextWeek(DateTime currday)
        { 
            DateTime weekStart = currday.AddDays(7).Date;
            DateTime weekEnd = weekStart.AddDays(6).Date;
            return myDb.Schedules.Where(x => x.Class_Date >= weekStart && x.Class_Date <= weekEnd && x.Schedule_Status == "ACTIVE").OrderBy(x => x.Start_Time).OrderBy(x => x.Class_Date);
        }

        public IEnumerable<Schedule> getSchedulesBackWeek(DateTime currday)
        {
            DateTime weekStart = currday.AddDays(-7).Date;
            DateTime weekEnd = currday.AddDays(-1).Date;
            return myDb.Schedules.Where(x => x.Class_Date >= weekStart && x.Class_Date <= weekEnd && x.Schedule_Status == "ACTIVE").OrderBy(x => x.Start_Time).OrderBy(x => x.Class_Date);
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
            string signedUp = getSignedUp(s.Schedule_Id).ToString();

            
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

            sched.Teacher_Id = np.Teacher_Id;
            sched.Class_Id = np.Class_Id;
            sched.Room_Id = np.Room_Id;
            sched.Start_Time = np.Start_Time;
            sched.Class_Date = np.Class_Date;
            sched.Schedule_Status = np.Schedule_Status;
            sched.Room = np.Room;

            myDb.SaveChanges();
        }

        public void DeleteSchedule(int id)
        {
            var or = myDb.Schedules.Where(x => x.Schedule_Id == id).Single();

            myDb.Schedules.Remove(or);

            myDb.SaveChanges();

        }

        public void RestoreSchedule(int id)
        {
            Schedule schedule = myDb.Schedules.Where(x => x.Schedule_Id == id).Single();

            schedule.Schedule_Status = "ACTIVE";

            myDb.SaveChanges();

        }

        public void CancelSchedule(int id)
        {
            Schedule schedule = myDb.Schedules.Where(x => x.Schedule_Id == id).Single();

            schedule.Schedule_Status = "CANCELLED";

            myDb.SaveChanges();

        }

        public void CancelledScheduleRefund(int id)
        {
            Schedule schedule = myDb.Schedules.Where(x => x.Schedule_Id == id).Single();
            List<Yoga_User> list = getScheduleSignUpList(id);
            foreach(Yoga_User u in list)
            {
                u.U_Tokens++;
                Class_Log cl = GetClass_LogsByUidAndSid(u.U_Id, id);
                cl.Log_Status = "CANCELLED";
            }

            myDb.SaveChanges();
        }

        public void RestoreScheduleRemoveUsers(int id)
        {
            Schedule schedule = myDb.Schedules.Where(x => x.Schedule_Id == id).Single();
            List<Yoga_User> list = getScheduleSignUpList(id);
            foreach (Yoga_User u in list)
            {
                Class_Log cl = GetClass_LogsByUidAndSid(u.U_Id, id);
                myDb.Class_Log.Remove(cl);
            }
            myDb.SaveChanges();
        }

        public List<Yoga_User> getScheduleSignUpList(int scheduleId)
        {
            IEnumerable<Class_Log> cl = myDb.Class_Log.Where(x => x.Schedule_Id == scheduleId).ToList();
            List<Yoga_User> yu = new List<Yoga_User>();
            foreach(Class_Log log in cl)
            {
                yu.Add(log.Yoga_User);
            }

            return yu.ToList();
        }


        public void ScheduleSignUp(int id)
        {

            var sched = myDb.Schedules.Where(x => x.Schedule_Id == id).Single();

            sched.Signed_Up++;
            
            myDb.SaveChanges();

        }

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

        public IEnumerable<Pass_Log> GetSaleReport(DateTime d1, DateTime d2) 
        {
            return myDb.Pass_Log.Where(x => x.Date_Purchased >= d1 && x.Date_Purchased <= d2);
        }



        public IEnumerable<Schedule> GetHoursWorkedReport(DateTime d1, DateTime d2, int userId)
        {

            return myDb.Schedules.Where(x => x.Class_Date >= d1 && x.Class_Date <= d2 && x.Teacher_Id == userId);

        }


        //

        //Attendance 
        public IEnumerable<Schedule> GetAttendanceDailySchedule(DateTime d1)
        {
            //IEnumerable<Schedule> 
            return myDb.Schedules.Where(x => x.Class_Date == d1);
        }

        public IEnumerable<Schedule> GetAttendanceDatesSchedule(DateTime d1, DateTime d2)
        {
            //IEnumerable<Schedule> 
            return myDb.Schedules.Where(x => x.Class_Date == d1);
        }

        //Attendance 

        public List<AttendanceDaily> GetAttendanceDailyReport(DateTime d1)
        {
            List<AttendanceDaily> attendanceList = new List<AttendanceDaily>();

            //IEnumerable<Schedule> 

            List<Schedule> schedList = myDb.Schedules.Where(x => x.Class_Date == d1).ToList();

            foreach (var sched in schedList)
            {
                // loop classes 
                var attended = myDb.Class_Log.Count(x => x.Schedule_Id == sched.Schedule_Id && x.Log_Status == "ATTENDED");

                var missed = myDb.Class_Log.Count(x => x.Schedule_Id == sched.Schedule_Id && x.Log_Status == "MISSED");

                AttendanceDaily ad = new AttendanceDaily();
                ad.scheduleId = sched.Schedule_Id;
                ad.startTime = sched.Start_Time;
                ad.SignUp = attended + missed;
                //
                ad.attended = attended;
                ad.missed = missed;

                var classe = getClass(sched.Class_Id);
                ad.className = classe.Class_Name;
                // class name 

                attendanceList.Add(ad);
            }


            return attendanceList;
        }


        public List<AttendanceDates> GetAttendanceDatesReport(DateTime d1, DateTime d2)
        {
            List<AttendanceDates> attendanceList = new List<AttendanceDates>();


            List<Schedule> schedList = myDb.Schedules.Where(x => x.Class_Date >= d1 && x.Class_Date <= d2).ToList();

            List<Class> classList = myDb.Classes.ToList();


            Class classt = new Class();

            ArrayList classArray = new ArrayList();


            foreach (var classe in classList)
            {

                AttendanceDates ad = new AttendanceDates();

                ad.classId = classe.Class_Id;
                ad.className = classe.Class_Name;
                ad.SignUp = 0;
                ad.attended = 0;
                ad.missed = 0;
                //AttendanceDates ad = new AttendanceDates();

                attendanceList.Add(ad);

            }


            foreach (var att in attendanceList)
            {

                foreach (var sched in schedList)
                {
                    // check if classes
                    if (att.classId == sched.Class_Id)
                    {


                        att.SignUp += sched.Signed_Up;

                        // loop class log 
                        int attended = myDb.Class_Log.Count(x => x.Schedule_Id == sched.Schedule_Id && x.Log_Status == "ATTENDED");

                        int missed = myDb.Class_Log.Count(x => x.Schedule_Id == sched.Schedule_Id && x.Log_Status == "MISSED");

                        att.attended += attended;
                        att.missed += missed;

                    }

                }
            }

            return attendanceList;

        }
         

    }
}
