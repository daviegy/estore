using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SON_eStore.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        public string Fname { get; set; }
        public string LName { get; set; }
        public string Name
        {
            get { return Fname + " " + LName; }
        }
        public string Gender { get; set; }
        public DateTime? Reg_date { get; set; }
        public string rolename { get; set; }
        public string rank { get; set; }
        public string status { get; set; }
        //[Index(IsUnique = true)]
        //[Column(TypeName = "VARCHAR")]
        //[StringLength(50)]
        public string staff_id { get; set; }
        public string dept_id { get; set; }
        public string dept_name { get; set; }
        public string unit_id { get; set; }
        public string staffType { get; set; }
        public string stateOffice { get; set; }
        public string regionalOffice { get; set; }
        public string unit_name { get; set; }
        [ForeignKey("dept_id")]
        public virtual Department dept { get; set; }
        [ForeignKey("unit_id")]
        public virtual dept_units dept_unit { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<category> category { get; set; }
        public DbSet<products> product { get; set; }
        public DbSet<state> state { get; set; }
        public DbSet<suppliers> supplier { get; set; }
        //public DbSet<store_requisition> store_requisition { get; set; }
        public DbSet<Department> department { get; set; }
        public DbSet<dept_units> dept_unit { get; set; }
        public DbSet<UsersActivitiesLog> usersActivitiesLog { get; set; }
        public DbSet<Stock_In_Items> stock_in_items { get; set; }
        public DbSet<store_requisitionTb> store_requisition { get; set; }
        public DbSet<cartViewModel> cart { get; set; }
        public DbSet<item_supplied_cart> item_supplied_cart { get; set; }
        //public DbSet<borrowcartViewModel> borrowcartViewModel { get; set; }
        public DbSet<conversionTable> conversionTable { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}