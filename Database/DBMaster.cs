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

        // User Get Methods 
        public void CreateUser()
        {
            YogaUser m = new YogaUser(); 
            Yoga_User y = new Yoga_User(YogaUser);

            y.Roles_Id = role;
            y.U_First_Name = fn;
            y.U_Last_Name = ln;
            y.U_Email = email;
            y.U_Password = password;
            y.U_Phone = phone;
            y.Availability = availability;
            y.U_Birthday = birthday;

            myDb.Yoga_User.Add(y);
        }

        public void UpdateUser(int id)
        {
           var u = myDb.Yoga_User.Where(x => x.U_Id == id).Single();
          
        }
    }
}
