using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SON_eStore.Models;
namespace SON_eStore.Models
{
    public class UserslogActivities
    {
      public   ApplicationDbContext db = new ApplicationDbContext();
        public   void loguserActivities(string username, string operation){
            if (!string.IsNullOrEmpty(username) )
            {
                var user = db.Users.FirstOrDefault(u => u.UserName == username);
                var logActivies = new UsersActivitiesLog();
                logActivies.name = user.Name;
                logActivies.username = user.UserName;
                logActivies.userRole = user.rolename;
                logActivies.operation = operation;
                logActivies.date = DateTime.UtcNow;
                db.usersActivitiesLog.Add(logActivies);
                 db.SaveChangesAsync();
               
            }
           
           
        }
    }
}