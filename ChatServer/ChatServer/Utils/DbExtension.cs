using Chat.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Utils
{
    static class DbExtension
    {
        public static bool SaveChangesEx(this AppDbContext db)
        {
            try
            {
                _ = db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
