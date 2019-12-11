using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Database
{
   

    public class DBMaster
    {
         yogadbEntities myDb = new yogadbEntities();
        

        // User Get Methods 
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
           var u = myDb.Yoga_User.Where(x => x.U_Email == email && x.U_Password == pass).Single();
            if(u == null)
            {
                return false;
            }
            else
            {
                return true;
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

        public IEnumerable<Yoga_User> getUserByRole(int role)
        {
            return myDb.Yoga_User.Where(x => x.Roles_Id == role);
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

        //Role Get Methods
        public string getRoleName(int id)
        {
            var r = myDb.Roles.Where(x => x.Roles_Id == id).Single();
            return r.Roles_Name;
        }

        // Room Get Methods
        public IEnumerable<Room> getRooms()
        {
            return myDb.Rooms.ToList();
        }

        public IEnumerable<Room> getRoomsByName(string name)
        {
            return myDb.Rooms.Where(x => x.Room_Name.Contains(name));
        }

        // Room Create/Update Methods 

        public void CreateRoom(Room r)
        {
            myDb.Rooms.Add(r);
        }

        public void UpdateRoom(int id)
        {
            var or = myDb.Rooms.Where(x => x.Room_Id == id).Single();

            Room nr = new Room();

            nr.Room_Name = or.Room_Name;
            nr.Room_Capacity = or.Room_Capacity;

            myDb.SaveChanges();
        }




        // Class Get Methods 
        public IEnumerable<Class> getClasses()
        {
            return myDb.Classes.ToList();
        }

        public IEnumerable<Class> getClassesByName(string name)
        {
            return myDb.Classes.Where(x => x.Class_Name.Contains(name));
        }

        // Room Create/Update Methods 

        public void CreateClass(Class c)
        {
            myDb.Classes.Add(c);
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





        //Promotion Get Methods

        public IEnumerable<Promotion> getPromotions()
        {
            return myDb.Promotions.ToList();
        }

        public IEnumerable<Promotion> getClassesByDiscount(int dis)
        {
            return myDb.Promotions.Where(x => x.Discount == dis);
        }

        public IEnumerable<Promotion> getClassesByExtraPasses(int extra)
        {
            return myDb.Promotions.Where(x => x.Num_Classes == extra);
        }

        //Promotion Create/Update Methods

        public void CreatePromotion(Promotion p)
        {
            myDb.Promotions.Add(p);
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





        //Class Passes Get Methods

        public IEnumerable<Class_Passes> getClassPasses()
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
        }

        public void UpdateClassPass(int id)
        {
            var ocp = myDb.Class_Passes.Where(x => x.Pass_Id == id).Single();

            Class_Passes ncp = new Class_Passes();

            ocp.Pass_Name = ncp.Pass_Name;
            ocp.Pass_Size = ncp.Pass_Size;
            ocp.Pass_Price = ncp.Pass_Price;
            ocp.Active = ncp.Active;

            myDb.SaveChanges();
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
        }




        //Schedule Get Methods
        public IEnumerable<Schedule> getSchedules()
        {
            return myDb.Schedules.ToList();
        }

        public IEnumerable<Schedule> getSchedulesNext7Days()
        {
            DateTime today = DateTime.Today;
            DateTime week = today.AddDays(7);
            return myDb.Schedules.Where(x => x.Class_Date >= today && x.Class_Date <= week);
        }
    }
}
