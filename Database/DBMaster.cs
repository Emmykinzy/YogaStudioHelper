using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
   

    class DBMaster
    {
        private static yogadbEntities myDb;

        public static IEnumerable<Yoga_User> getCustomer()
        {
            return myDb.Yoga_User.ToList();
        }


    }
}
