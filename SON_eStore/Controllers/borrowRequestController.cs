using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SON_eStore.Models;
using System.Globalization;
using System.Web.Http.Cors;
namespace SON_eStore.Controllers
{
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "StoreAdmin,StoreKeeper")]
    public class borrowRequestController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserslogActivities ulog = new UserslogActivities();
        Random rd = new Random();
        [HttpPost]
        //[Route("api/storeRequest/{id}")]
        public IHttpActionResult createRequest([FromBody] cartViewModel model)
        {
            try
            {
                var logInUserName = RequestContext.Principal.Identity.Name;
                store_requisitionTb stRequest = new store_requisitionTb();
                var cart = db.cart.Where(c => c.cart_id == model.cart_id).ToList();

                if (cart.Count() > 0)
                {
                    var request_order_no = string.Concat("OR", rd.Next(1000));
                    foreach (var item in cart)
                    {
                        stRequest.R_order_no = request_order_no;
                        stRequest.S_R_V_No = item.s_r_v_no;
                        stRequest.product_id = item.item_id;
                        stRequest.product_name = item.item_name;
                        stRequest.conversion_value = item.conversion_value;
                        stRequest.Requested_qty_unit = item.Requested_qty_unit;
                        stRequest.Requested_qty_unit_value = item.Requested_qty_unit_value;
                        stRequest.item_base_unit = item.item_base_unit;
                        stRequest.qty_requested = item.Qty_Requested;
                        stRequest.qty_allocated = item.Qty_Allocated;
                        stRequest.reqst_staff_id = item.staff_id;
                        stRequest.reqst_staff_name = item.staff_name;
                        stRequest.state_office = item.State;
                        stRequest.regional_office = item.Region;
                        stRequest.department = item.dept_name;
                        stRequest.dept_id = item.dept_id;
                        stRequest.unit = item.unit_name;
                        stRequest.unit_id = item.unit_id;
                        stRequest.request_type = item.Request_type;
                        stRequest.request_status = "borrow";
                        stRequest.created_by = logInUserName;
                        stRequest.Request_dt = (item.requested_date != null) ? DateTime.ParseExact(item.requested_date.ToString(), "d/M/yyyy", CultureInfo.InvariantCulture) : DateTime.UtcNow;
                        stRequest.Created_Date = DateTime.UtcNow.Date;
                        db.store_requisition.Add(stRequest);
                        db.SaveChanges();
                        var item_in_stock = db.product.Find(item.item_id);
                        if (item_in_stock != null)
                        {
                            item_in_stock.total_item_allocated_pending_approval += item.Qty_Allocated;
                            item_in_stock.current_stock_pending_approval = item_in_stock.opening_stock_qty - item_in_stock.total_item_allocated_pending_approval;
                            db.SaveChanges();
                        }
                    }
                    foreach (var item in cart)
                    {                        // delete this item from cart
                        var ctItem = db.cart.Find(item.id);
                        db.cart.Remove(ctItem);
                        db.SaveChanges();
                    }
                    //this line return the order id of the newly created store request.
                    ulog.loguserActivities(logInUserName, "Created new store request");
                    // var getOrderid = db.store_requisition.FirstOrDefault(order => order.R_order_no == request_order_no).R_order_no;
                    return Ok();
                }
                return Content(HttpStatusCode.NoContent, "no content in cart");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpGet]
        [Route("api/borrowRequest/borrow_list")]
        public IHttpActionResult borrow_list()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var borrow_approval_RQlist = db.store_requisition.Where(s => s.request_status.ToLower() == "borrow")
                .GroupBy(k => k.R_order_no)
                .Select(g => new
                {
                    orderid = g.Key,
                    request_borrow_items = g.Select(c => new storeRequestListByORDERID_vm
                    {
                        orderid = c.R_order_no,
                        request_status = c.request_status,
                        request_type = c.request_type,
                        requesting_state = c.state_office,
                        requesting_region = c.regional_office,
                        requesting_dept = c.department,
                        unit = c.unit,
                        reqst_staff_name = c.reqst_staff_name,
                        srv = c.S_R_V_No,
                        total_item_requested = db.store_requisition.Where(t => t.R_order_no == c.R_order_no && t.request_status.ToLower() == "borrow").Count(),
                        request_date = c.Request_dt,
                        created_date = c.Created_Date
                    }).Distinct(),
                    //     request_status = 
                }).ToList();
            ulog.loguserActivities(logInUserName, "View list of borrow request list");
            List<storeRequestListByORDERID_vm> storeborrow = new List<storeRequestListByORDERID_vm>();
            foreach (var p in borrow_approval_RQlist)
            {
                storeborrow.AddRange(p.request_borrow_items);
            }
            return Ok(storeborrow);
        }
        [HttpGet]
        [Route("api/borrowRequest/{id}/orderp")]
        public IHttpActionResult getItemsByOrder(string id)
        {
            if (id != null)
            {
                //int Id = Convert.ToInt32(id);
                var items = db.store_requisition.Where(o => o.R_order_no == id && o.request_status.ToLower() == "borrow");
                return Ok(items.ToList());
            }
            return NotFound();
        }
        [HttpGet]
        [Route("api/borrowRequest/{id}/markAsFreshRq")]
        public IHttpActionResult markAsFreshRequest(string id)
        {
            if (id != null)
            {
                var items = db.store_requisition.Where(o => o.R_order_no == id && o.request_status.ToLower() == "borrow");
                if (items.Count() > 0)
                {
                    foreach (var item in items)
                    {
                        var store_rq_item = db.store_requisition.Find(item.R_ID);
                        store_rq_item.request_status = "fresh";
                       
                    }
                    db.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            return NotFound();
        }
    }
}
