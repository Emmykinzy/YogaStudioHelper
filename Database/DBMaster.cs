using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Database
{
   

    class DBMaster
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

    }
}
