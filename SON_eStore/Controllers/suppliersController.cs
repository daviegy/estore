using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SON_eStore.Models;
using SON_eStore.Customclasses;
using System.Web.Http.Cors;
namespace SON_eStore.Controllers
{
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "StoreAdmin,StoreKeeper")]
    public class suppliersController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Random rd = new Random();
        UserslogActivities ulog = new UserslogActivities();
        public class SuppliersViewModel
        {
            public string id { get; set; }
            public string supplier_name { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string state { get; set; }
            public string city { get; set; }
        }
        [System.Web.Http.HttpGet]
        public IHttpActionResult getSupplier()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var ct = db.supplier.ToList();
            ulog.loguserActivities(logInUserName, "User requested for supplier's List");
            return Ok(ct);
        }
        [System.Web.Http.HttpGet]
        public IHttpActionResult getSupplier(string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (id != null)
                {
                    var ct = db.supplier.Find(id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User requested for: '" + ct.supplier_name + "'");
                        return Ok(ct);
                    }
                    else
                    {
                        ulog.loguserActivities(logInUserName, "Supplier '" + id + "' not found");
                        return Content(HttpStatusCode.NotFound, "Supplier not found");
                    }
                }
                else
                {
                    ulog.loguserActivities(logInUserName, "Supplier '" + id + " 'not found");
                    return Content(HttpStatusCode.NotFound, "Supplier not found");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        [Route("api/suppliers/{id}/items")]
        public IHttpActionResult getitemSupplied(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var sup_items = db.stock_in_items.Where(s => s.supplier_id == id).ToList();
                if (sup_items.Count() > 0)
                {
                    return Ok(sup_items);
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, "No item found");
                }
            }
            return NotFound();
        }
        [System.Web.Http.HttpPut]
        public HttpResponseMessage updateSupplier([FromBody]SuppliersViewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {

                if (model.supplier_name != null && model.id != null)
                {
                    //IDictionary<string, string> values = JsonConvert.DeserializeObject<IDictionary<string, string>>(data);
                    string supplier_name = model.supplier_name;
                    var ct = db.supplier.Find(model.id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User update the details of supplier with id: '" + ct.id + "'");
                        ct.supplier_name = model.supplier_name;
                        ct.phone_no = model.phone; ;
                        ct.email = model.email;
                        ct.address = model.address;
                        ct.state = model.state;
                        ct.reg_date = DateTime.UtcNow;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Record is updated successfully!");
                    }
                    else
                    {
                        ulog.loguserActivities(logInUserName, "supplier details update fail");
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "supplier details update fail");
                    }
                }
                ulog.loguserActivities(logInUserName, "supplier details update fail");
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "supplier details update fail");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult createSupplier([FromBody]SuppliersViewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (model.supplier_name != null)
                {
                    //IDictionary<string, string> values = JsonConvert.DeserializeObject<IDictionary<string, string>>(data);
                  
                    var ct = new suppliers();
                    if (ct != null)
                    {
                      
                        ct.id = string.Concat("S-" + rd.Next(1000));
                        ct.supplier_name = model.supplier_name;
                        ct.phone_no = model.phone; ;
                        ct.email = model.email;
                        ct.address = model.address;
                        ct.state = model.state;
                        ct.city = model.city;
                        ct.reg_date = DateTime.UtcNow;
                        db.supplier.Add(ct);
                        db.SaveChanges();
                        ulog.loguserActivities(logInUserName, "Register new supplier named: '" + model.supplier_name + "'");
                        return Content(HttpStatusCode.OK, "Supplier has been successfully registered!");
                    }
                    else
                    {
                       // ulog.loguserActivities(logInUserName, "supplier details not registered");
                        return Content(HttpStatusCode.NotFound, "supplier details not registered");
                    }
                }
             //S   ulog.loguserActivities(logInUserName, "supplier details not registered");
                return Content(HttpStatusCode.NotFound, "ensure that all field values are supplied");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "supplier details not registered: " + ex);
            }

        }
        [HttpDelete]
        [Authorize(Roles = "StoreAdmin")]
        public IHttpActionResult delSupplier([FromUri] string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (id != null)
            {
                var ct = db.supplier.Find(id);
               // var pr = db.stock_in_items.Where(p => p.supplier_id == ct.id).Count();
                if (ct.stock_in_items.Count() <= 0)
                {
                    ulog.loguserActivities(logInUserName, "Deleted '" + ct.supplier_name + "' from list of suppliers ");
                    db.supplier.Remove(ct);
                    db.SaveChanges();

                    return Ok();
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Supplier has one or more items associated with it, kindly delete the items firsts.");
                }

            }
            return NotFound();
        }
    }
}
