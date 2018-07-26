using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
    [Authorize(Roles = "StoreAdmin")]
    public class RolesController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserslogActivities ulog = new UserslogActivities();
        Random rd = new Random();
        public class roleViewModel
        {
            public string id { get; set; }
            public string Name { get; set; }
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getRole()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var ct = db.Roles.ToList();
            ulog.loguserActivities(logInUserName, "Requested for  user roles list");
            var response = Request.CreateResponse(HttpStatusCode.OK, ct);
            response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { 
            NoCache = true,
            NoStore = true,
            MaxAge = TimeSpan.FromSeconds(20)
            };
            return response;
        }
        [System.Web.Http.HttpGet]
        public IHttpActionResult getRole(string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (id != null)
                {
                    var ct = db.Roles.Find(id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User requested for: '" + ct.Name + "'");
                        return Ok(ct);
                    }
                    else
                    {
                       // ulog.loguserActivities(logInUserName, "R '" + id + "' not found");
                        return Content(HttpStatusCode.NotFound, "Role not found");
                    }
                }
                else
                {
                  //  ulog.loguserActivities(logInUserName, "Category '" + id + " 'not found");
                    return Content(HttpStatusCode.NotFound, "Role not found");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
        [System.Web.Http.HttpPut]
        public HttpResponseMessage updateCategory([FromBody]roleViewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {

                if (model.Name != null && model.id != null)
                {
                    //IDictionary<string, string> values = JsonConvert.DeserializeObject<IDictionary<string, string>>(data);
                    string r_name = model.Name;
                    var ct = db.Roles.Find(model.id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User Changed Role name: '" + ct.Name + "' to '" + r_name + "'");
                        ct.Name = r_name;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Record is updated successfully!");
                    }
                    else
                    {
                        //ulog.loguserActivities(logInUserName, "Category update fail");
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "role update fail");
                    }
                }
                ulog.loguserActivities(logInUserName, "role update fail");
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "role Update fail");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult createCategory([FromBody]roleViewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (model.Name != null)
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                    if (!roleManager.RoleExists(model.Name))
                    {
                        object roleresult = roleManager.Create(new IdentityRole(model.Name));
                    }
                    ulog.loguserActivities(logInUserName, "Added new role with name: '" + model.Name + "' ");
                    return Content(HttpStatusCode.OK, "role has been successfully created");

                }
                else
                    return Content(HttpStatusCode.BadRequest, "Operation fail: ");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Operation fail: " + ex);
            }

        }
        [HttpDelete]       
        public IHttpActionResult delCategory([FromUri] string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (id != null)
            {
                var ct = db.Roles.Find(id);            
                if (ct!=null)
                {
                    ulog.loguserActivities(logInUserName, "User deleted '" + ct.Name + "' Role ");
                    db.Roles.Remove(ct);
                    db.SaveChanges();

                    return Ok();
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "role does not exist.");
                }

            }
            return NotFound();
        }
    }
}
