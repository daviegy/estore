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
    public class itemsController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserslogActivities ulog = new UserslogActivities();
        Random rd = new Random();

        [HttpGet]
        public IHttpActionResult getitems()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var model = db.product.Select(i => new itemsViewModel
            {
                id = i.id,
                category_name = i.category.category_name,
                product_name = i.product_name,
                desc = i.p_descripition,
                qtyAvailable = i.opening_stock_qty,
                item_base_unit = i.item_base_unit,
                qtyReorderAlertValue = i.stock_reorder_alert_qty
            });
            if (model != null)
            {
                ulog.loguserActivities(logInUserName, "Requested for items list");
                return Ok(model.ToList());
            }
            return Ok();
        }
        [HttpGet]
        public IHttpActionResult getitems(string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (!string.IsNullOrEmpty(id))
            {
                var model = db.product.FirstOrDefault(i => i.id == id);
                if (model != null)
                {
                    ulog.loguserActivities(logInUserName, "Requested for '" + model.product_name + "'");
                    return Ok(model);
                }

            }
            return Content(HttpStatusCode.NotFound, "The requested item is not found");

        }
        [Route("api/items/in_stock")]
        [HttpGet]
        public IHttpActionResult getItemsInStock()
        {
            var items = db.product.Where(p => p.opening_stock_qty > 0).ToList();
            return Ok(items);
        }
        [HttpPut]
        public IHttpActionResult updateItem([FromBody]itemsViewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (model.id != null && model.product_name != null && model.catid != null && model.qtyAvailable >= 0 && model.qtyAvailable >= model.qtyReorderAlertValue)
                {
                    var items = db.product.Find(model.id);
                    if (items != null)
                    {
                        ulog.loguserActivities(logInUserName,
                            "Update item name from '" + items.product_name + "' to '" + model.product_name + "' and item quantity from '" + items.opening_stock_qty + "' to '" + model.qtyAvailable + "' the following");
                        items.product_name = model.product_name;
                        items.p_descripition = model.desc;
                        items.serial_no = model.serial_no;
                        items.cat_id = model.catid;
                        items.opening_stock_qty = model.qtyAvailable;
                        items.stock_reorder_alert_qty = model.qtyReorderAlertValue;
                        items.item_base_unit = model.item_base_unit;
                        items.current_stock_pending_approval = items.opening_stock_qty - items.total_item_allocated_pending_approval;
                        items.unitPrice = model.unitPrice;
                        db.SaveChanges();
                        return Ok();
                    }

                }
                return Content(HttpStatusCode.NotFound, "Item with id '" + model.id + "'not found");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }
        [HttpPost]
        public IHttpActionResult addItem([FromBody]itemsViewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (model.is_Item_In_Store == "yes")
                {
                    if (model.product_name != null && model.catid != null && model.qtyAvailable >= 0 && model.qtyAvailable >= model.qtyReorderAlertValue)
                    {
                        var items = new products();
                        items.id = "P-" + rd.Next(1000);
                        items.product_name = model.product_name;
                        items.p_descripition = model.desc;
                        items.serial_no = model.serial_no;
                        items.cat_id = model.catid;
                        items.opening_stock_qty = model.qtyAvailable;
                        items.item_base_unit = model.item_base_unit;
                        items.total_item_allocated_pending_approval = 0;
                        items.current_stock_pending_approval = items.opening_stock_qty - items.total_item_allocated_pending_approval;
                        items.stock_reorder_alert_qty = model.qtyReorderAlertValue;
                        items.unitPrice = model.unitPrice;
                        db.product.Add(items);
                        db.SaveChanges();
                        ulog.loguserActivities(logInUserName,
                              "Added new item '" + items.product_name + "'");
                        return Ok();
                    }
                }
                else
                {
                    if (model.product_name != null && model.catid != null && model.qtyReorderAlertValue >= 0)
                    {
                        var items = new products();
                        items.id = "P-" + rd.Next(1000);
                        items.product_name = model.product_name;
                        items.p_descripition = model.desc;
                        items.serial_no = model.serial_no;
                        items.cat_id = model.catid;
                        items.opening_stock_qty = model.qtyAvailable;
                        items.item_base_unit = model.item_base_unit;
                        items.total_item_allocated_pending_approval = 0;
                        items.current_stock_pending_approval = items.opening_stock_qty - items.total_item_allocated_pending_approval;
                        items.stock_reorder_alert_qty = model.qtyReorderAlertValue;
                        items.unitPrice = model.unitPrice;
                        db.product.Add(items);
                        db.SaveChanges();
                        ulog.loguserActivities(logInUserName,
                              "Added new item '" + items.product_name + "'");
                        return Ok();
                    }
                }

                return Content(HttpStatusCode.NotAcceptable, "Some items field are wrongly filled");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "StoreAdmin")]
        public IHttpActionResult delItem([FromUri] string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (id != null)
            {

                var pr = db.product.Find(id);
                if (pr.stock_in_items.Count() <= 0)
                {
                    ulog.loguserActivities(logInUserName, "User deleted '" + pr.product_name + "' item from the item list. ");
                    db.product.Remove(pr);
                    db.SaveChanges();
                    return Ok();
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "item cannot be deleted!");
                }

            }
            return NotFound();
        }

        [HttpGet]
        [Authorize(Roles = "StoreAdmin,StoreKeeper")]
        [Route("api/items/frequentlyRequested")]
        public IHttpActionResult frqitems()
        {
            var f_items = db.store_requisition.Where(s => s.request_status == "Approved")
                .GroupBy(i => i.product_id).Select(s => new
                {
                    item_id = s.Key,
                    items = s.Select(c => new frequenttlyRequestedItem
                    {
                       // item_id = c.product_id,
                        item_name = c.product_name,
                        totalRequested = db.store_requisition.Where(p => p.product_id == c.product_id && p.request_status =="Approved").Sum(t => t.qty_allocated)
                    }).Distinct()
                }).ToList().Take(10);

            List<frequenttlyRequestedItem> fqItems = new List<frequenttlyRequestedItem>();
            foreach (var fitems in f_items)
            {
                //List<object> x = new List<object>();
                
                fqItems.AddRange(fitems.items.ToList());
            }
            if(fqItems.Count()>0)
            { return Ok(fqItems.ToList()); }
            else { return BadRequest(); }
        }
    }
}
