using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SON_eStore.Customclasses;
using SON_eStore.Models;
using System.Web.Http.Cors;
using System.Globalization;
using System.Data.Entity.Validation;
namespace SON_eStore.Controllers
{

    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "StoreAdmin,StoreKeeper")]
    public class storeSuppliesController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Random rd = new Random();
        UserslogActivities ulog = new UserslogActivities();
        public class StoreSupplyViewModel
        {
            public string stock_id { get; set; }
            public string supplier_id { get; set; }
            public string supplier_name { get; set; }
            public string product_id { get; set; }
            public string product_name { get; set; }
            public int qty_supplied { get; set; }
            public string qty_denomination { get; set; }
            public int qty_supplied_in_base_unit { get; set; }
            public string item_base_unit { get; set; }
            public string supplied_date { get; set; }
            
            public decimal? unitprice { get; set; }
            public decimal? totalAmount { get; set; }
            //public DateTime? dt { get; set; }

            public object s_r_v_no { get; set; }
        }
        [System.Web.Http.HttpGet]
        public IHttpActionResult getSupplies()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var ct = db.stock_in_items.ToList();
            ulog.loguserActivities(logInUserName, "Requested for store items supplies list");
            return Ok(ct);
        }
        [System.Web.Http.HttpGet]
       
        public IHttpActionResult getSupplies(string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (id != null)
                {
                    var ct = db.stock_in_items.Find(id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "Requested for store supplied item: '" + ct.product_name + "'");
                        return Ok(ct);
                    }
                    else
                    {
                       // ulog.loguserActivities(logInUserName, "Supplier '" + id + "' not found");
                        return Content(HttpStatusCode.NotFound, "Supplies not found");
                    }
                }
                else
                {
                  //  ulog.loguserActivities(logInUserName, "Supplies '" + id + " 'not found");
                    return Content(HttpStatusCode.NotFound, "Supplies not found");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [System.Web.Http.HttpPut]
        public HttpResponseMessage updateSupplies([FromBody]StoreSupplyViewModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {

                if (model.s_r_v_no != null &&model.supplier_id != null && model.product_id != null && model.qty_supplied > 0 )
                {
                    //IDictionary<string, string> values = JsonConvert.DeserializeObject<IDictionary<string, string>>(data);
                    string supplier_name = db.supplier.Find(model.supplier_id).supplier_name;
                    var p = db.product.Find(model.product_id);
                    var ct = db.stock_in_items.Find(model.stock_id);
                    if (ct != null)
                    {
                        ulog.loguserActivities(logInUserName, "Update the details of Store supply item : '" + ct.product_name+ "'");
                        ct.supplier_name = supplier_name;
                        ct.supplier_id = model.supplier_id;
                        ct.product_id = model.product_id;
                        ct.product_name = p.product_name;
                        ct.qty_supplied = model.qty_supplied;
                        ct.unitPrice = model.unitprice;
                        ct.totalAmount= model.totalAmount;
                        ct.supplied_date = DateTime.ParseExact(model.supplied_date, "d/M/yyyy", CultureInfo.InvariantCulture);
                        db.SaveChanges();
                        p.opening_stock_qty += ct.qty_supplied;
                        p.current_stock_pending_approval = p.opening_stock_qty - p.total_item_allocated_pending_approval;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Store supplied item detail is updated successfully!");
                    }
                    else
                    {
                       // ulog.loguserActivities(logInUserName, "supplier details update fail");
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "supplies details update fail");
                    }
                }
             //   ulog.loguserActivities(logInUserName, "supplier details update fail");
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "item was never received into store");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult createSupplies([FromBody]item_supplied_cart model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            try
            {
                if (model.s_r_v_no != null)
                {
                    //IDictionary<string, string> values = JsonConvert.DeserializeObject<IDictionary<string, string>>(data);
                   
                    var ct = new Stock_In_Items();
                    var cart = db.item_supplied_cart.Where(c => c.s_r_v_no == model.s_r_v_no).ToList();
                    if (cart.Count()>0)
                    {
                        
                        foreach (var item in cart)
                        {
                           //ct.stock_id = string.Concat("ST-" + item.s_r_v_no);
                            ct.s_r_v_no = item.s_r_v_no;
                            ct.supplier_name = item.supplier;
                            ct.supplier_id = item.supplier_id;
                            ct.product_id = item.item_id;
                            ct.product_name = item.item_name;
                            ct.qty_supplied = item.qtySupplied;
                            ct.qty_denomination = item.qty_denomination;
                            ct.qty_supplied_in_base_unit = item.qty_supplied_in_base_unit;
                            ct.item_base_unit = item.item_base_unit;
                            ct.unitPrice = item.unit_price;
                            ct.Amount = item.total_amount_per_item;
                            //ct.totalAmount += ct.Amount;
                            ct.supplied_date = DateTime.ParseExact(item.supplied_date, "d/M/yyyy", CultureInfo.InvariantCulture);
                            ct.Created_date = DateTime.UtcNow.Date;
                            ct.Recieved_by = logInUserName;
                            db.stock_in_items.Add(ct);
                            db.SaveChanges();
                            var p = db.product.Find(item.item_id);
                            if (p != null)
                            {
                                p.opening_stock_qty += item.qty_supplied_in_base_unit;
                                p.current_stock_pending_approval = p.opening_stock_qty - p.total_item_allocated_pending_approval;
                                db.SaveChanges();
                            }                           
                        }
                        foreach (var item in cart)
                        {                        // delete this item from cart
                            var ctItem = db.item_supplied_cart.Find(item.id);
                            db.item_supplied_cart.Remove(ctItem);
                            db.SaveChanges();
                        }
                        ulog.loguserActivities(logInUserName, "Newly supplied items added to store");
                        return Ok(model.s_r_v_no);
                    }
                    else
                    {
                        // ulog.loguserActivities(logInUserName, "supplier details not registered");
                        return Content(HttpStatusCode.NotFound, "supplied items not added to store.");
                    }
                }
                //ulog.loguserActivities(logInUserName, "supplier details not registered");
                return Content(HttpStatusCode.NotFound,"");
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "StoreAdmin")]
        public IHttpActionResult delSupplies([FromUri] string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (id != null)
            {
                var ct = db.stock_in_items.Find(id);
                // var pr = db.stock_in_items.Where(p => p.supplier_id == ct.id).Count();
                if (ct!=null)
                {
                    ulog.loguserActivities(logInUserName, "Deleted '"+ct.product_name+"' from list of store supplies ");
                    db.stock_in_items.Remove(ct);
                    db.SaveChanges();
                    return Ok();
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, "item was never received into store");
                }

            }
            return NotFound();
        }
    }
}
