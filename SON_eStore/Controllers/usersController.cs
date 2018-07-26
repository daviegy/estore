using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SON_eStore.Models;
using System.Web.Http.Cors;
using WebApi.OutputCache.V2;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
namespace SON_eStore.Controllers
{
    //[Authorize]
    [EnableCors("*", "*", "*")]
    [IgnoreCacheOutput]
    public class usersController : ApiController
    {
        UserslogActivities ulog = new UserslogActivities();
        ApplicationDbContext db = new ApplicationDbContext();
        [HttpGet]
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public IHttpActionResult getUsers()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var u = db.Users.Select(s => new StaffViewModel
            {
                id = s.Id,
                Name = s.Fname + " " + s.LName,
                //phone = s.PhoneNumber,
                //email = s.Email,
                //staff_id = s.staff_id,
                dept_name = s.dept_name,
                unit_name = s.unit_name,
                username = s.UserName,
                rank = s.rank,
                rolename = s.rolename
            });
            if (u != null)
            {
                ulog.loguserActivities(logInUserName, "User requested for registered users List");
                return Ok(u);
            }
            else
                return BadRequest();

        }
        [HttpGet]
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        [Route("api/users/getStaff")]
        public IHttpActionResult getStaff()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var u = db.Users.Where(r => r.rolename != "StoreAdmin" && r.rolename != "StoreKeeper").Select(s => new StaffViewModel
            {
                id = s.Id,
                Name = s.Fname + " " + s.LName,
                stateOffice = s.stateOffice,
                regionalOffice = s.regionalOffice,
                dept_name = s.dept_name,
                unit_name = s.unit_name,
                username = s.UserName,
                rank = s.rank,
                rolename = s.rolename
            });
            if (u != null)
            {
                ulog.loguserActivities(logInUserName, "User requested for registered users List");
                return Ok(u);
            }
            else
                return BadRequest();

        }
        [HttpGet]
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        [Route("api/users/geteStoreUsers")]
        public IHttpActionResult geteStoreUsers()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var u = db.Users.Where(r=>r.rolename == "StoreAdmin" ||r.rolename=="StoreKeeper").Select(s => new StaffViewModel
            {
                id = s.Id,
                Name = s.Fname + " " + s.LName,
                //phone = s.PhoneNumber,
                //email = s.Email,
                //staff_id = s.staff_id,
                dept_name = s.dept_name,
                unit_name = s.unit_name,
                username = s.UserName,
                rank = s.rank,
                rolename = s.rolename
            });
            if (u != null)
            {
                ulog.loguserActivities(logInUserName, "User requested for registered users List");
                return Ok(u);
            }
            else
                return BadRequest();

        }
        [HttpGet]
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        public IHttpActionResult getuser(string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var u = db.Users.Find(id);
            if (u != null)
            {
                var us = new StaffViewModel
                {
                    id = u.Id,
                    Fname = u.Fname,
                    Lname = u.LName,
                   staffType = u.staffType,
                   stateOffice = u.stateOffice,
                   regionalOffice = u.regionalOffice,
                    dept_name = u.dept_name,
                    unit_name = u.unit_name,
                    username = u.UserName,
                    role_id =(!string.IsNullOrEmpty(u.rolename))?u.Roles.First(r => r.UserId == u.Id).RoleId:null,
                    dept_id = u.dept_id,
                    unit_id = u.unit_id,
                    rank = u.rank
                };
                ulog.loguserActivities(logInUserName, "User requested for registered users List");
                return Ok(us);
            }
            else
                return NotFound();

        }
        [HttpGet]
        [Route("api/users/approvers")]
        [Authorize(Roles = "StoreAdmin")]
        public IHttpActionResult getApprovers() {
            var approver = db.Users.Where(r => r.rolename == "StoreAdmin").ToList();
            if (approver.Count() > 0)
                return Ok(approver);
            else
                return Content(HttpStatusCode.NotFound, "No one has the right to approve this request yet.");
        }
        //[HttpPost]
        //[Route("api/users/register")]
        //public IHttpActionResult registerUser(StaffViewModel model)
        //{
        //    try
        //    {
        //        var logInUserName = RequestContext.Principal.Identity.Name;
        //        var d = (model.dept_id != null) ? db.department.Find(model.dept_id) : null;
        //        var u = (model.unit_id != null) ? db.dept_unit.Find(model.unit_id) : null;
        //        var r = (model.role_id != null) ? db.Roles.Find(model.role_id) : null;
        //        var uname = string.Concat(model.Fname, model.Lname.Substring(0, 1));
        //        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        //        PasswordHasher pw = new PasswordHasher();
        //        //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        //        //if (!roleManager.RoleExists("StoreAdmin"))
        //        //{
        //        //    object roleresult = roleManager.Create(new IdentityRole("StoreAdmin"));
        //        //}
        //        //if (!roleManager.RoleExists("StoreKeeper"))
        //        //{
        //        //    object roleresult = roleManager.Create(new IdentityRole("StoreKeeper"));
        //        //}
        //        var user = new ApplicationUser();
        //        user.UserName = uname;
        //        user.Fname = model.Fname;
        //        user.LName = model.Lname;
        //        // PhoneNumber = model.phone,
        //        user.Reg_date = DateTime.UtcNow;
        //        user.dept_name = (d != null) ? d.dept_name : null;
        //        user.dept_id = model.dept_id;
        //        user.unit_id = model.unit_id;
        //        user.unit_name = (u != null) ? u.unit_name : null;
        //        //  EmailConfirmed = true,
        //        user.rolename = (r != null) ? r.Name : null;
        //        user.rank = model.rank;
        //        user.status = "Active";
        //        user.PasswordHash = pw.HashPassword("password");
        //        user.SecurityStamp = Guid.NewGuid().ToString();
        //        db.Users.Add(user);
        //        db.SaveChanges();
        //       /// var UserRole = userManager.GetRoles(user.Id);
        //        if (!string.IsNullOrEmpty(user.rolename))
        //        {
        //            var addToRole = userManager.AddToRole(user.Id, user.rolename);
        //        }
        //        ulog.loguserActivities(logInUserName, "Registered new user");
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.BadRequest,ex.Message);
        //    }
           
          
        //}
        [HttpPut]
        [Authorize(Roles = "StoreAdmin")]
        public IHttpActionResult updateUser([FromBody]StaffViewModel model)
        {
           // var logInUserName = RequestContext.Principal.Identity.Name;
            var u = db.Users.Find(model.id);
            if (u != null)
            {
                var d = (model.dept_id != null) ? db.department.Find(model.dept_id) : null;
                var du = (model.unit_id != null) ? db.dept_unit.Find(model.unit_id) : null;
                var r = (model.role_id != null) ? db.Roles.Find(model.role_id) : null;               
                u.Fname = model.Fname;
                u.LName = model.Lname;
                u.stateOffice = model.stateOffice;
                u.regionalOffice = model.regionalOffice;
                u.staffType = model.staffType;           
                u.dept_name = (d != null) ? d.dept_name : null;
                u.dept_id = (d!=null)?d.id : null;
                u.unit_id = (du!=null)?du.id:null;
                u.unit_name = (du != null) ? du.unit_name : null;
                u.rolename = (r != null) ? r.Name : null;
                u.rank = model.rank;
                db.SaveChanges();
                return Ok();
            }
            else
                return NotFound();

        }
        [HttpDelete]
        [Authorize(Roles = "StoreAdmin")]
        public IHttpActionResult deleteUser([FromUri]string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var u = db.Users.Find(id);
            if (u != null)
            {
                ulog.loguserActivities(logInUserName, "Deleted '"+u.Fname+" "+u.LName+"' from the users List");
                db.Users.Remove(u);
                db.SaveChanges();              
                return Ok(u);
            }
            else
                return NotFound();
        }
        //[Authorize(Roles = "StoreAdmin")]
        [AllowAnonymous]
        [HttpPost]
        [Route("api/users/{id}/CheckStaffID")]
        public IHttpActionResult checkStaffId(string id)
        {
            var getStaff = db.Users.Where(s => s.staff_id == id).Select(u => new StaffViewModel
            {
                id = u.Id,
                staff_id = u.staff_id
            });

            if (getStaff.Count() > 0)
                return Content(HttpStatusCode.OK, getStaff);
            else
                return Content(HttpStatusCode.NotFound, getStaff);
        }
        // [Authorize(Roles="StoreAdmin")]
        [AllowAnonymous]
        [HttpPost]
        [Route("api/users/{id}/checkEmail")]
        public IHttpActionResult checkEmail(string id)
        {
            var getStaff = db.Users.Where(s => s.Email == id).Select(u => new StaffViewModel
            {
                id = u.Id,
                email = u.Email
            });

            if (getStaff.Count() > 0)
                return Content(HttpStatusCode.OK, getStaff);
            else
                return Content(HttpStatusCode.NotFound, getStaff);
        }
        [HttpGet]
        [Route("api/users/{id}/StateStaffs")]
        public IHttpActionResult getStaffsByStateName(string id)
        {
            if (id != null)
            {
                var staff = db.Users.Where(c => c.stateOffice == id).Select(u => new StaffViewModel
                {
                    stateOffice = u.stateOffice,                  
                    Name = u.Fname + " " + u.LName,
                    staff_id = u.staff_id,
                    id = u.Id
                }).ToList();
                if (staff.Count > 0)
                    return Ok(staff);
                else
                    return Content(HttpStatusCode.NotFound, "No staff found for this state office");
            }
            else return Content(HttpStatusCode.NotFound, "No staff found for this state office");
        }
        [HttpGet]
        [Route("api/users/{id}/regionalStaffs")]
        public IHttpActionResult getStaffsByRegionalOffice(string id)
        {
            if (id != null)
            {
                var staff = db.Users.Where(c => c.regionalOffice == id).Select(u => new StaffViewModel
                {
                    regionalOffice = u.regionalOffice,
                    Name = u.Fname + " " + u.LName,
                    staff_id = u.staff_id,
                    id = u.Id
                }).ToList();
                if (staff.Count > 0)
                    return Ok(staff);
                else
                    return Content(HttpStatusCode.NotFound, "No staff found for this regional office");
            }
            else return Content(HttpStatusCode.NotFound, "No staff found for this regional office");
        }
    }
}
