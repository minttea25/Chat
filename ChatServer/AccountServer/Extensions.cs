using AccountServer.DB;
using ChatSharedDb;
using Microsoft.EntityFrameworkCore;

namespace AccountServer
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

        public static bool SaveChangesEx(this SharedDbContext db)
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
