using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using SON_eStore.Models;
using Newtonsoft.Json;
using SON_eStore.Customclasses;
namespace SON_eStore.Controllers
{

    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "StoreAdmin,StoreKeeper")]
    public class categoryController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserslogActivities ulog = new UserslogActivities();
        Random rd = new Random();
        public class catviewModel {
            public string id { get; set; }
            public string category_name { get; set; }
        }
         
        [System.Web.Http.HttpGet]
        public IHttpActionResult getCategoryList() {
            var logInUserName = RequestContext.Principal.Identity.Name;            
            var ct = db.category.ToList();
            ulog.loguserActivities(logInUserName,"User requested for Category List");
            return Ok(ct);
        }
       [System.Web.Http.HttpGet]
        public IHttpActionResult getCategoryList(string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name; 
            try
            {
                if (id != null)
                {
                    var ct = db.category.Find(id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User requested for: '"+ ct.category_name+"'");
                        return Ok(ct);
                    }
                    else
                    {
                        ulog.loguserActivities(logInUserName, "Category '" + id + "' not found");
                        return Content(HttpStatusCode.NotFound, "Category not found");
                    }
                }
                else
                {
                    ulog.loguserActivities(logInUserName, "Category '"+id+" 'not found");
                    return Content(HttpStatusCode.NotFound, "Category not found");  
                }
                                 
                
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }
        [HttpGet][Route("api/category/{id}/items")]
       public IHttpActionResult getItemsByCatgory(string id)
       {
           if (id != null)
           {
               var items = db.product.Where(c => c.cat_id == id).Select(u => new itemsViewModel { 
                category_name= u.category.category_name,
                product_name = u.product_name,
                id = u.id,
                qtyAvailable = u.opening_stock_qty
               }).ToList();
               if (items.Count > 0)
                    return Ok(items);
               else
                   return Content(HttpStatusCode.NotFound, "No item found for this category");
           }
           else return Content(HttpStatusCode.NotFound, "No item found for this category");
       }
        [System.Web.Http.HttpPut]
        public HttpResponseMessage updateCategory([FromBody]catviewModel model)
       {
           var logInUserName = RequestContext.Principal.Identity.Name;         
            try
            {
               
                if (model.category_name != null && model.id != null)
                {
                    //IDictionary<string, string> values = JsonConvert.DeserializeObject<IDictionary<string, string>>(data);
                    string category_name = model.category_name;
                    var ct = db.category.Find(model.id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "User Changed Category name: '"+ct.category_name+"' to '"+category_name+"'");
                        ct.category_name = category_name;
                        db.SaveChanges();                        
                        return Request.CreateResponse(HttpStatusCode.OK, "Record is updated successfully!");
                    }
                    else
                    {
                        ulog.loguserActivities(logInUserName, "Category update fail");
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Category update fail");
                    }
                }
                ulog.loguserActivities(logInUserName, "Category update fail");
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Category Update fail");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
            
        }
       [System.Web.Http.HttpPost]
        public IHttpActionResult createCategory([FromBody]catviewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;   
            try {
                if (model.category_name != null)
                {
                    var ct = new category();
                    ct.category_name = model.category_name;
                    ct.id = string.Concat("C-", rd.Next(1000));
                    db.category.Add(ct);
                    db.SaveChanges();
                    ulog.loguserActivities(logInUserName, "New Category with name: '" + ct.category_name+"' created ");
                    return Content(HttpStatusCode.OK, "Category has been successfully created");

                }
                else
                    return Content(HttpStatusCode.BadRequest, "Operation fail: ");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Operation fail: "+ex);
            }
          
        }
       [HttpDelete]
       [Authorize(Roles = "StoreAdmin")]
       public IHttpActionResult delCategory([FromUri] string id)
       {
           var logInUserName = RequestContext.Principal.Identity.Name;   
           if (id != null)
           {
               var ct = db.category.Find(id);
               var pr = db.product.Where(p => p.cat_id == ct.id).Count();
               if (ct.products.Count() <= 0)
               {
                   ulog.loguserActivities(logInUserName, "User deleted '" + ct.category_name + "' category ");
                   db.category.Remove(ct);
                   db.SaveChanges();

                   return Ok();
               }
               else
               {
                   return Content(HttpStatusCode.BadRequest, "Category has one or more items associated with it, kindly delete the items firsts.");
               }
              
           }
           return NotFound();
       }
    }
}
