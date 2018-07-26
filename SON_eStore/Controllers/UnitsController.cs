using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SON_eStore.Customclasses;
using SON_eStore.Models;
using System.Web.Http.Cors;
namespace SON_eStore.Controllers
{
    /// <summary>
    /// Department units
    /// </summary>
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "StoreAdmin,StoreKeeper")]
    public class UnitsController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Random rd = new Random();
        UserslogActivities ulog = new UserslogActivities();
        [System.Web.Http.HttpGet]
        public IHttpActionResult getUnit()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var ct = db.dept_unit.Select(u => new UnitsviewModel { 
            dept_name = u.department.dept_name,
            id = u.id,
            unit_name = u.unit_name
            }).ToList();
            ulog.loguserActivities(logInUserName, "User requested for department's units");
            return Ok(ct);
        }
        [System.Web.Http.HttpGet]
        public IHttpActionResult getUnit(string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (id != null)
                {
                    var ct = db.dept_unit.Find(id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User requested for: '" + ct.unit_name + "'");
                        return Ok(ct);
                    }
                    else
                    {
                        ulog.loguserActivities(logInUserName, "Unit '" + id + "' not found");
                        return Content(HttpStatusCode.NotFound, "Unit not found");
                    }
                }
                else
                {
                    ulog.loguserActivities(logInUserName, "Unit '" + id + " 'not found");
                    return Content(HttpStatusCode.NotFound, "Unit not found");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }              
        [System.Web.Http.HttpPost]
        public IHttpActionResult createUnit([FromBody]UnitsviewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (model.unit_name != null && model.dept_id != null)
                {
                    var dp = db.department.Find(model.dept_id);

                    var ct = new dept_units();
                    ct.unit_name = model.unit_name;
                    ct.id = string.Concat("DU-", rd.Next(1000));
                    ct.dept_id = model.dept_id;
                    db.dept_unit.Add(ct);
                    db.SaveChanges();
                    ulog.loguserActivities(logInUserName, "Added: '" + ct.unit_name + "' to '"+dp.dept_name+"' department.");
                    return Content(HttpStatusCode.OK, "Unit has been successfully created");

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
        public HttpResponseMessage updateUnit([FromBody]UnitsviewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (model.unit_name != null && model.dept_id != null && model.id != null)
                {
                   var dp = db.department.Find(model.dept_id);

                    var ct = db.dept_unit.Find(model.id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User Changed unit name: '" + ct.unit_name + "' to '" + model.unit_name + "' and assign it to '"+dp.dept_name+"' department");
                        ct.unit_name = model.unit_name;
                        ct.dept_id = model.dept_id;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "unit details has been updated successfully!");
                    }
                    else
                    {
                      //  ulog.loguserActivities(logInUserName, "Units update fail");
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Units update fail");
                    }
                }
              //  ulog.loguserActivities(logInUserName, "Units update fail");
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Units Update fail");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpDelete]
        [Authorize(Roles = "StoreAdmin")]
        public IHttpActionResult delunits([FromUri] string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (id != null)
            {
                var ct = db.dept_unit.Find(id);
             //   var pr = db..Where(p => p.cat_id == ct.id).Count();
                if (ct.users.Count() <= 0)
                {
                    ulog.loguserActivities(logInUserName, "Deleted '" + ct.unit_name + "' unit. ");
                    db.dept_unit.Remove(ct);
                    db.SaveChanges();
                    return Ok();
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Units cannot be deleted, because it has one or more staffs associated with it, Kindly delete the staffs firsts.");
                }

            }
            return NotFound();
        }
    }
}
