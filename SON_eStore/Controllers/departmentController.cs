using SON_eStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SON_eStore.Controllers
{
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "StoreAdmin,StoreKeeper")]
    public class departmentController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserslogActivities ulog = new UserslogActivities();
        Random rd = new Random();
        [System.Web.Http.HttpGet]
        public IHttpActionResult getDept()
        {
            
            var logInUserName = RequestContext.Principal.Identity.Name;
            var ct = db.department.Select(d => new deptviewModel() { 
            dept_name = d.dept_name,
            id = d.id,
            reg_staff_size = d.users.Count(),
            reg_unit_size = d.dept_units.Count(),            
            }).ToList();
            ulog.loguserActivities(logInUserName, "User requested for department List");
            return Ok(ct);
        }
        [System.Web.Http.HttpGet]
        public IHttpActionResult getDept(string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (id != null)
                {
                    var ct = db.department.Find(id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User requested for: '" + ct.dept_name + "'");
                        return Ok(ct);
                    }
                    else
                    {
                        ulog.loguserActivities(logInUserName, "department '" + id + "' not found");
                        return Content(HttpStatusCode.NotFound, "department not found");
                    }
                }
                else
                {
                    ulog.loguserActivities(logInUserName, "department '" + id + " 'not found");
                    return Content(HttpStatusCode.NotFound, "department not found");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        [Route("api/department/{id}/units")]
        public IHttpActionResult getUnitsByDept(string id)
        {
            if (id != null)
            {
                var units = db.dept_unit.Where(c => c.dept_id == id).Select(u => new UnitsviewModel
                {
                    dept_id = u.department.id,
                    dept_name = u.department.dept_name,
                    unit_name = u.unit_name,
                    id = u.id,                    
                }).ToList();
                if (units.Count > 0)
                    return Ok(units);
                else
                    return Ok();
            }
            else return Content(HttpStatusCode.NotFound, "No unit found for this department");
        }
        [HttpGet]
        [Route("api/department/{id}/staffs")]
        public IHttpActionResult getStaffsByDeptId(string id)
        {
            if (id != null)
            {
                var staff = db.Users.Where(c => c.dept_id == id).Select(u => new StaffViewModel
                {
                    dept_id = u.dept.id,
                    dept_name = u.dept.dept_name,
                    Name = u.Fname +" "+ u.LName ,
                    staff_id = u.staff_id,
                    id = u.Id
                }).ToList();
                if (staff.Count > 0)
                    return Ok(staff);
                else
                    return Content(HttpStatusCode.NotFound, "No staff found for this department");
            }
            else return Content(HttpStatusCode.NotFound, "No staff found for this department");
        }        
        [System.Web.Http.HttpPost]
        public IHttpActionResult createDept([FromBody]deptviewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (model.dept_name != null)
                {
                    var ct = new Department();
                    ct.dept_name = model.dept_name;
                    ct.id = string.Concat("D-", rd.Next(1000));
                    db.department.Add(ct);
                    db.SaveChanges();
                    ulog.loguserActivities(logInUserName, "New department with name: '" + ct.dept_name + "' created ");
                    return Content(HttpStatusCode.OK, "Department has been successfully created");

                }
                else
                    return Content(HttpStatusCode.BadRequest, "Operation fail: ");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Operation fail: " + ex);
            }

        }
        [System.Web.Http.HttpPut]
        public HttpResponseMessage updateDept([FromBody]deptviewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {

                if (model.dept_name != null)
                {
                    //IDictionary<string, string> values = JsonConvert.DeserializeObject<IDictionary<string, string>>(data);
                    
                    var ct = db.department.Find(model.id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User Changed department name: '" + ct.dept_name + "' to '" + model.dept_name + "'");
                        ct.dept_name = model.dept_name;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Record is updated successfully!");
                    }
                    else
                    {
                        ulog.loguserActivities(logInUserName, "department update fail");
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "department update fail");
                    }
                }
                ulog.loguserActivities(logInUserName, "department update fail");
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "department Update fail");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpDelete]
        [Authorize(Roles = "StoreAdmin")]
        public IHttpActionResult delDept([FromUri] string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (id != null)
            {
                var ct = db.department.Find(id);
                
                if (ct.dept_units.Count() <= 0 && ct.users.Count()<=0)
                {                   
                    ulog.loguserActivities(logInUserName, "User deleted '" + ct.dept_name + "' department. ");
                    db.department.Remove(ct);
                    db.SaveChanges();

                    return Ok();
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Department has one or more units and staffs associated with it, kindly delete the units and staffs firsts.");
                }

            }
            return NotFound();
        }
    }
}
