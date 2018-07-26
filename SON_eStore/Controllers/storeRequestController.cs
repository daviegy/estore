using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SON_eStore.Models;
using SON_eStore.Customclasses;
using System.Web.Http.Cors;
using System.Globalization;
namespace SON_eStore.Controllers
{
    [Authorize(Roles="StoreAdmin,StoreKeeper")]
    [EnableCors("*", "*", "*")]
    public class storeRequestController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserslogActivities ulog = new UserslogActivities();
        Random rd = new Random();
        public class approveModel
        {
            public string orderid { get; set; }
            public string approverid { get; set; }
        }
        [HttpGet]
        [Route("api/storeRequest/validateSRV/{srv}")]
        public IHttpActionResult validateSRV(string srv)
        {
            if (!string.IsNullOrEmpty(srv))
            {
                var check_srv = db.store_requisition.Where(s => s.S_R_V_No == srv && (s.request_status.ToString().ToLower() == "fresh" || s.request_status.ToString().ToLower() == "pending" || s.request_status.ToString().ToLower() == "approved"));
                if (check_srv.Count() > 0)
                {
                    return Content(HttpStatusCode.BadRequest, "Store requisition number is not allowed, because it has already been used to process item request before. ");
                }
            }           
            return Ok();
        }
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
                        stRequest.isLabRequest = item.isLabRequest;
                        stRequest.regional_office = item.Region;
                        stRequest.department = item.dept_name;
                        stRequest.dept_id = item.dept_id;
                        stRequest.unit = item.unit_name;
                        stRequest.unit_id = item.unit_id;
                        stRequest.request_type = item.Request_type;
                        stRequest.request_status = "Fresh";
                        stRequest.created_by = logInUserName;
                        stRequest.Request_dt = (item.requested_date!=null)?DateTime.ParseExact(item.requested_date.ToString(), "d/M/yyyy", CultureInfo.InvariantCulture):DateTime.UtcNow;
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
        [Route("api/storeRequest/Fresh_request")]
        public IHttpActionResult getFreshRequest()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var fresh_rq = db.store_requisition.Where(s => s.request_status.ToLower() == "fresh")
                .GroupBy(k => k.R_order_no)
                .Select(g => new
                {
                    orderid = g.Key,
                    fresh_request = g.Select(c => new storeRequestListByORDERID_vm
                    {
                        orderid = c.R_order_no,
                        //request_status = c.request_status,
                        request_type = c.request_type,
                        requesting_state = c.state_office,
                        requesting_region = c.regional_office,
                        requesting_dept = c.department,
                        unit = c.unit,
                        reqst_staff_name = c.reqst_staff_name,
                        total_item_requested = db.store_requisition.Where(t => t.R_order_no == c.R_order_no && t.request_status.ToLower() == "fresh").Count(),
                        request_date = c.Request_dt,
                        created_date = c.Created_Date,
                        srv = c.S_R_V_No
                    }).Distinct(),
                    //     request_status = 
                }).ToList().Distinct();
            ulog.loguserActivities(logInUserName, "View list of fresh request");
            List<storeRequestListByORDERID_vm> storefresh = new List<storeRequestListByORDERID_vm>();
            foreach (var p in fresh_rq)
            {
                storefresh.AddRange(p.fresh_request);
            }
            return Ok(storefresh);
        }

        [HttpGet][Route("api/storeRequest/pending_approval")]
        public IHttpActionResult getPendingRequest()
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            var pending_approval_RQlist = db.store_requisition.Where(s => s.request_status.ToLower() == "pending")
                .GroupBy(k => k.R_order_no)
                .Select(g => new 
                {
                    orderid = g.Key,
                    request_pending = g.Select(c => new storeRequestListByORDERID_vm{
                    orderid = c.R_order_no,
                    request_status =c.request_status,
                    request_type = c.request_type,
                    requesting_state = c.state_office,
                    requesting_region = c.regional_office,
                    requesting_dept = c.department,
                        unit = c.unit,
                       reqst_staff_name = c.reqst_staff_name, 
                        total_item_requested = db.store_requisition.Where(t=>t.R_order_no == c.R_order_no && t.request_status.ToLower() == "pending").Count(),
                    request_date = c.Request_dt,
                    created_date = c.Created_Date,
                    srv = c.S_R_V_No
                    }).Distinct(),
               //     request_status = 
                }).ToList();
            ulog.loguserActivities(logInUserName, "View list of pending request");
            List<storeRequestListByORDERID_vm> storepending = new List<storeRequestListByORDERID_vm>();
            foreach (var p in pending_approval_RQlist)
            {
                storepending.AddRange(p.request_pending);
            }
            return Ok(storepending);
        }
       
        [HttpGet]
        public IHttpActionResult getItemsByid(string id)
        {
            if (id != null)
            {
                int Id = Convert.ToInt32(id) ;
                var items = db.store_requisition.Find(Id);                
                return Ok(items);
            }
            return NotFound();
        }
        [HttpGet][Route("api/storeRequest/{id}/orderp")]
        public IHttpActionResult getItemsByOrder(string id)
        {
            if (id != null)
            {
                //int Id = Convert.ToInt32(id);
                var items = db.store_requisition.Where(o=>o.R_order_no == id && o.request_status.ToLower() == "pending");
                return Ok(items.ToList());
            }
            return NotFound();
        }
        [HttpGet]
        [Route("api/storeRequest/{id}/orderf")]
        public IHttpActionResult get_FreshItem_Request(string id)
        {
            if (id != null)
            {
                //int Id = Convert.ToInt32(id);
                var items = db.store_requisition.Where(o => o.R_order_no == id && o.request_status.ToLower() == "fresh");
                int c = items.Count();
                return Ok(items.ToList());
            }
            return NotFound();
        }
        [HttpPut]
        public IHttpActionResult updateRequistion([FromBody] store_requisitionTb model)
        {
            if (model != null)
            {
                var logInUserName = RequestContext.Principal.Identity.Name;
                var strq = db.store_requisition.Find(model.R_ID);               
                if(model.qty_allocated < strq.qty_requested){
                    ulog.loguserActivities(logInUserName, "updated the quantity of item allocated for '" + strq.product_name + "' from '" + strq.qty_allocated + "' to '" + model.qty_allocated + "'");
                    int diffrence = strq.qty_allocated - model.qty_allocated;
                    strq.qty_allocated = model.qty_allocated;
                    strq.qty_supplied = strq.qty_allocated;
                    db.SaveChanges();
                    var item = db.product.Find(strq.product_id);
                    item.total_item_allocated_pending_approval -= diffrence;
                    db.SaveChanges();
                    return Ok();
                }
                return Content(HttpStatusCode.BadRequest, "Quantity Allocated cannot be more than Quantity Requested");               
            }
            return BadRequest();
        }
        [HttpPost]
        [Route("api/storeRequest/approveRequest")]
        public IHttpActionResult approveRequest([FromBody] approveModel model)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (model.orderid != null && model.approverid != null)
            {
                var store_request = db.store_requisition.Where(o => o.R_order_no == model.orderid).ToList();
                if (store_request.Count() > 0)
                {
                    foreach (var items in store_request)
                    {
                        var approver = db.Users.Find(model.approverid).Name;
                        var item = db.store_requisition.Find(items.R_ID);
                        var update_StoreRecord = db.product.Find(item.product_id);
                        update_StoreRecord.opening_stock_qty -= item.qty_allocated;
                        update_StoreRecord.total_item_allocated_pending_approval -= item.qty_allocated;
                        update_StoreRecord.current_stock_pending_approval = update_StoreRecord.opening_stock_qty - update_StoreRecord.total_item_allocated_pending_approval;
                        db.SaveChanges();
                        item.approve_by = approver;
                        item.request_status = "Approved";
                        item.Approve_dt = DateTime.UtcNow;
                        item.Item_Qty_In_Store_After_Approval = update_StoreRecord.opening_stock_qty;
                        db.SaveChanges();
                    }
                  //  ulog.loguserActivities(logInUserName, "");
                    return Ok(model.orderid);
                }
            }
            return Content(HttpStatusCode.NotFound,"");
           
        }
        [HttpDelete]
        public IHttpActionResult deleteItemInRequestOrder([FromUri]string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (id != null)
            {
                int ID = Convert.ToInt32(id);
                var str = db.store_requisition.Find(ID);
                if (str != null)
                {
                    ulog.loguserActivities(logInUserName, "Deleted '"+str.product_name+"' from list of requested items by '"+str.department+"'");
                    var item = db.product.Find(str.product_id);
                    item.total_item_allocated_pending_approval -= str.qty_allocated;
                    item.current_stock_pending_approval = item.opening_stock_qty - item.total_item_allocated_pending_approval;
                    db.SaveChanges();
                    db.store_requisition.Remove(str);
                    db.SaveChanges();
                }
                return Ok();

            }
            return Content(HttpStatusCode.NotFound, "item not found");
        }
        [Route("api/storeRequest/{id}/order")]
        [HttpDelete]
        public IHttpActionResult deleteItembyorderid([FromUri]string id)
        {
            var logInUserName = RequestContext.Principal.Identity.Name;
            if (id != null)
            {
                //int ID = Convert.ToInt32(id);
                var str = db.store_requisition.Where(o=>o.R_order_no == id);
                if (str != null)
                {
                   // ulog.loguserActivities(logInUserName, "Deleted borrowed item requested by '" + str.product_name + "' from list of requested items by '" + str.department + "'");
                    foreach (var items in str)
                    {
                        var item = db.product.Find(items.product_id);
                        item.total_item_allocated_pending_approval -= items.qty_allocated;
                        item.current_stock_pending_approval = item.opening_stock_qty - item.total_item_allocated_pending_approval;
                       // db.SaveChanges();
                        db.store_requisition.Remove(items);
                      
                    }
                    db.SaveChanges();
                  
                }
                return Ok();

            }
            return Content(HttpStatusCode.NotFound, "item not found");
        }
    }
}
