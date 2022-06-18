using Server.DB;
using SharedDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    // Extensions 메소드는 무조건 public static 으로
    public static class Extensions
    {
        public static bool SaveChangesEx(this AppDbContext db)
        {
            try
            {
                db.SaveChanges();
                return true;
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"쉬벌롬들아{ex}");
                return false;
            }
        }

        public static bool SaveChangesEx(this SharedDbContext db)
        {
            try
            {
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }





}
