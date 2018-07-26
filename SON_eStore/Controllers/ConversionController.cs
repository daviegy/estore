using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SON_eStore.Models;
using System.Web.Http.Cors;

namespace SON_eStore.Controllers
{
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "StoreAdmin,StoreKeeper")]
    public class ConversionController : ApiController
    {
         ApplicationDbContext db = new ApplicationDbContext();
        UserslogActivities ulog = new UserslogActivities();
        public IHttpActionResult getConversion()
        {
            var ctab = db.conversionTable.ToList();
            return Ok(ctab);
        }
        public IHttpActionResult getConversion(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int c_id = Convert.ToInt32(id);
                var ctab = db.conversionTable.Find(c_id);
                return Ok(ctab);
            }
            return NotFound();
          
            
        }
        [HttpGet]
        [Route("api/Conversion/conversion_by_items/{item_id}/{m_unit}")]
        public IHttpActionResult conversion_by_items(string item_id, string m_unit)
        {
            if (!string.IsNullOrEmpty(item_id) && !string.IsNullOrEmpty(m_unit))
            {
               // int c_id = Convert.ToInt32(id);
                var ctab = db.conversionTable.FirstOrDefault(c=>c.item_id == item_id && c.master_unit == m_unit);
                if(ctab != null)
                {
                    return Ok(ctab.base_unit_value);
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, "No conversion found for this item");
                }
                
            }
            return BadRequest();


        }
        [HttpPost]
        public IHttpActionResult addCoversion([FromBody] conversionTable model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                var cTab = new conversionTable();
                var p = db.product.Find(model.item_id);
                if (model != null)
                {                    
                    cTab.item_id = model.item_id;
                    cTab.item_name = p.product_name;
                    cTab.master_unit = model.master_unit;
                    cTab.master_unit_value = model.master_unit_value;
                    cTab.base_unit = model.base_unit;
                    cTab.base_unit_value = model.base_unit_value;
                    //validate that a particular master unit for a single item is not duplicated.
                    var ctb = db.conversionTable.Where(i=>i.item_id == model.item_id);
                    if (ctb.Count() > 0)
                    {
                        foreach(var m_unit in ctb)
                        {
                            if(m_unit.master_unit == model.master_unit)
                            {
                                return Content(HttpStatusCode.BadRequest, "You cannot have duplicate of '" + model.master_unit + "' for '" + p.product_name + " in the conversion table, kindly, check the table to edit or delete previous conversion to '"+model.master_unit+"', incase you want to make changes.'");
                            }
                        }
                    }
                    db.conversionTable.Add(cTab);
                    db.SaveChanges();
                    ulog.loguserActivities(logInUserName, "Added new item conversion ");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.ToString());
            }
        }
        [HttpPut]
        public IHttpActionResult updateConversion([FromBody] conversionTable model)
        {
            try {
                if (!string.IsNullOrEmpty(model.id.ToString()))
                {
                    var ctab = db.conversionTable.Find(model.id);
                    ctab.base_unit_value = model.base_unit_value;
                    ctab.master_unit = model.master_unit;
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch(Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.InnerException.ToString());
            }
        }

        [HttpDelete]
        [Authorize(Roles = "StoreAdmin")]
        public IHttpActionResult delConversion ([FromUri] string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (!string.IsNullOrEmpty(id))
            {
                int c_id = Convert.ToInt32(id);
                var ctab = db.conversionTable.Find(c_id);
                db.conversionTable.Remove(ctab);
                db.SaveChanges();
                ulog.loguserActivities(logInUserName, "Deleted a single conversion for an item from the conversion table.");
                return Ok();
            
            }
            return NotFound();
        }
    }
}
