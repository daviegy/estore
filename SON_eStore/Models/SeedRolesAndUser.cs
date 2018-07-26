using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SON_eStore.Models
{
    public class SeedRolesAndUser
    {
        public static void Seed(ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            PasswordHasher pw = new PasswordHasher();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists("StoreAdmin"))
            {
                object roleresult = roleManager.Create(new IdentityRole("StoreAdmin"));
            }
            if (!roleManager.RoleExists("StoreKeeper"))
            {
                object roleresult = roleManager.Create(new IdentityRole("StoreKeeper"));
            }
            string userName = "Admin";
            string password = "Admin";
            string fname = "admin";
            string lname = "admin";
            ApplicationUser user = userManager.FindByName(userName);
            
            if ((user == null))
            {
                var u = new ApplicationUser();
                u.UserName = userName;
                u.Fname = fname;
                u.LName = lname;
                u.PasswordHash = pw.HashPassword(password);
                u.SecurityStamp = Guid.NewGuid().ToString();
                context.Users.Add(u);
                context.SaveChanges();
                user = userManager.FindByName(userName);
                if (user != null)
                {
                   var addtorole =  userManager.AddToRole(user.Id, "StoreAdmin");
                }
               
            }
        }

    }
}